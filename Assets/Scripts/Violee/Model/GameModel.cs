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
        MyFSM<EGameState> gameFsm = new ();
        bool isIdle => gameFsm.IsState(EGameState.Idle);
        bool isPlaying => gameFsm.IsState(EGameState.Playing);
        protected void Start()
        {
            Binder.From(gameFsm.GetState(EGameState.Playing)).OnUpdate(dt =>
            {
                MapModel.TickPlayerVisit(PlayerModel.Instance.transform.position);
                PlayerModel.Tick(dt);
            });
            Binder.From(gameFsm.GetState(EGameState.Playing)).OnExit(PlayerModel.OnExitPlaying);
            
            
            MapModel.StartGenerateFunc.Guard += () => isIdle || isPlaying;
            MapModel.DijkstraFunc.Guard += () => isIdle;
            MapModel.OnBeginGenerate += () => gameFsm.ChangeState(EGameState.GeneratingMap);
            MapModel.OnEndGenerate += () => gameFsm.ChangeState(EGameState.Idle);
            MapModel.OnBeginDij += () =>
            {
                gameFsm.ChangeState(EGameState.GeneratingMap);
                return Task.CompletedTask;
            };
            MapModel.OnEndDij += pos3D =>
            {
                gameFsm.ChangeState(EGameState.Playing);
                PlayerModel.OnEnterPlaying(pos3D);
            };
            
            Binder.Update(_ =>
            {
                if (Input.GetKeyDown(KeyCode.R))
                    MapModel.StartGenerateFunc.TryInvoke();
            });
            
            gameFsm.ChangeState(EGameState.Idle);
            Binder.Update(gameFsm.Update);
        }
    }
}