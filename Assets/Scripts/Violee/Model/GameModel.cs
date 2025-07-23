using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    public enum EGameState
    {
        Idle,
        GeneratingMap,
        Playing,
        WatchingMap,
    }
    public class GameModel : Singleton<GameModel>
    {
        [ShowInInspector]
        MyFSM<EGameState> gameFSM = new ();
        bool isIdle => gameFSM.IsState(EGameState.Idle);
        bool isPlaying => gameFSM.IsState(EGameState.Playing);
        protected void Start()
        {
            Binder.From(gameFSM.GetState(EGameState.Playing)).OnUpdate(dt =>
            {
                MapModel.TickPlayerVisit(PlayerModel.Instance.transform.position);
                PlayerModel.Tick(dt);
            });
            Binder.From(gameFSM.GetState(EGameState.Playing)).OnExit(PlayerModel.OnExitPlaying);
            
            
            MapModel.StartGenerateFunc.Guard += () => isIdle || isPlaying;
            MapModel.DijkstraFunc.Guard += () => isIdle;
            MapModel.OnBeginGenerate += () => gameFSM.ChangeState(EGameState.GeneratingMap);
            MapModel.OnEndGenerate += () => gameFSM.ChangeState(EGameState.Idle);
            MapModel.OnBeginDij += () =>
            {
                gameFSM.ChangeState(EGameState.GeneratingMap);
                return Task.CompletedTask;
            };
            MapModel.OnEndDij += pos3D =>
            {
                gameFSM.ChangeState(EGameState.Playing);
                PlayerModel.OnEnterPlaying(pos3D);
            };
            
            Binder.Update(_ =>
            {
                if (Input.GetKeyDown(KeyCode.R))
                    MapModel.StartGenerateFunc.TryInvoke();
            });
            
            gameFSM.ChangeState(EGameState.Idle);
            Binder.Update(gameFSM.Update);
        }
    }
}