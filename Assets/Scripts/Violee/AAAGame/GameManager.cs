using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee.View;

namespace Violee
{
    public enum EGameState
    {
        Idle,
        GeneratingMap,
        Playing,
        WatchingMap,
    }
    public class GameManager : Singleton<GameManager>
    {
        [ShowInInspector]
        MyFSM<EGameState> gameFsm = new ();
        bool isIdle => gameFsm.IsState(EGameState.Idle);
        bool isPlaying => gameFsm.IsState(EGameState.Playing);
        protected void Start()
        {
            Binder.From(gameFsm.GetState(EGameState.Playing)).OnUpdate(dt =>
            {
                BoxModelManager.TickPlayerVisit(PlayerModel.Instance.transform.position);
                PlayerModel.Tick(dt);
            });
            Binder.From(gameFsm.GetState(EGameState.Playing)).OnExit(PlayerModel.OnExitPlaying);
            
            
            BoxModelManager.StartGenerateFunc.Guard += () => isIdle || isPlaying;
            BoxModelManager.DijkstraFunc.Guard += () => isIdle;
            BoxModelManager.OnBeginGenerate += () => gameFsm.ChangeState(EGameState.GeneratingMap);
            BoxModelManager.OnEndGenerate += () => gameFsm.ChangeState(EGameState.Idle);
            BoxModelManager.OnBeginDij += () =>
            {
                gameFsm.ChangeState(EGameState.GeneratingMap);
                return Task.CompletedTask;
            };
            BoxModelManager.OnEndDij += pos3D =>
            {
                gameFsm.ChangeState(EGameState.Playing);
                PlayerModel.OnEnterPlaying(pos3D);
            };
            
            Binder.Update(_ =>
            {
                if (Input.GetKeyDown(KeyCode.R))
                    BoxModelManager.StartGenerateFunc.TryInvoke();
            });
            
            gameFsm.ChangeState(EGameState.Idle);
            Binder.Update(gameFsm.Update);
        }
    }
}