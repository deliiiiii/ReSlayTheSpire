using System;
using System.Diagnostics.CodeAnalysis;
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
        protected override void Awake()
        {
            base.Awake();
            playerModel = PlayerModel.Instance;
            boxModelManager = BoxModelManager.Instance;
            
            PlayingState.OnUpdate(dt =>
            {
                boxModelManager.TickPlayerVisit(playerModel.transform.position);
                playerModel.Tick(dt);
            });
            PlayingState.OnExit(playerModel.OnExitPlaying);
            
            
            boxModelManager.GenerateStream.Where(_ => isIdle || isPlaying);
            boxModelManager.GenerateStream.OnBegin += _ => gameFsm.ChangeState(EGameState.GeneratingMap);
            boxModelManager.GenerateStream.OnEnd += _ => gameFsm.ChangeState(EGameState.Idle);
            
            boxModelManager.DijkstraStream.Where(_ => isIdle);
            boxModelManager.DijkstraStream.OnBegin += _ => gameFsm.ChangeState(EGameState.GeneratingMap);
            boxModelManager.DijkstraStream.OnEnd += pair =>
            {
                gameFsm.ChangeState(EGameState.Playing);
                playerModel.OnEnterPlaying(pair.Item2);
            };
            
            gameFsm.ChangeState(EGameState.Idle);
            Binder.Update(gameFsm.Update);
        }

        [ShowInInspector]
        static MyFSM<EGameState> gameFsm = new ();
        static bool isIdle => gameFsm.IsState(EGameState.Idle);
        static bool isPlaying => gameFsm.IsState(EGameState.Playing);
        public readonly BindDataState GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        public readonly BindDataState PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        PlayerModel playerModel = null!;
        BoxModelManager boxModelManager = null!;
    }
}