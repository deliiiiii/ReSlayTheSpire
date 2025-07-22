using System;
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

        // TODO start比其他Awake晚。要不要控制一下初始化顺顺序？
        protected void Start()
        {
            Binder.From(gameFSM.GetState(EGameState.Playing)).OnUpdate(_ => MapModel.TickPlayerVisit());
            
            MapModel.StartGenerateFunc.Guard += () => isIdle || isPlaying;
            MapModel.DijkstraFunc.Guard += () => isIdle;
            MapModel.OnBeginGenerate += () => gameFSM.ChangeState(EGameState.GeneratingMap);
            MapModel.OnEndGenerate += () => gameFSM.ChangeState(EGameState.Idle);
            MapModel.OnBeginDij += async () =>
            {
                gameFSM.ChangeState(EGameState.GeneratingMap);
                await Task.CompletedTask;
            };
            MapModel.OnEndDij += _ => gameFSM.ChangeState(EGameState.Playing);
            
            Binder.Update(_ =>
            {
                if (Input.GetKeyDown(KeyCode.R))
                    MapModel.StartGenerateFunc.TryInvoke();
            });
            Binder.Update(gameFSM.Update);
            
            gameFSM.ChangeState(EGameState.Idle);
        }
    }
}