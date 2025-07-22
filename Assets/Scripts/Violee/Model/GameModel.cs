using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee.Player;

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
        protected void Start()
        {
            Binder.From(gameFSM.GetState(EGameState.Playing)).OnUpdate(_ => MapModel.TickPlayerVisit());
            Binder.From(gameFSM.GetState(EGameState.Playing)).OnExit(PlayerModel.OnExitPlaying);
            
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
            Binder.Update(gameFSM.Update);
        }
    }
}