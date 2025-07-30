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
        [ShowInInspector]
        static MyFSM<EGameState> gameFsm = new ();
        static bool isIdle => gameFsm.IsState(EGameState.Idle);
        static bool isPlaying => gameFsm.IsState(EGameState.Playing);
        public static readonly BindDataState GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        public static readonly BindDataState PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        [field: MaybeNull] PlayerModel playerModel => field ??= PlayerModel.Instance;
        protected void Start()
        {
            PlayingState.OnUpdate(dt =>
            {
                BoxModelManager.TickPlayerVisit(playerModel.transform.position);
                playerModel.Tick(dt);
            });
            PlayingState.OnExit(playerModel.OnExitPlaying);
            
            
            BoxModelManager.GenerateStream.Where(_ => isIdle || isPlaying);
            BoxModelManager.GenerateStream.OnBegin += _ => gameFsm.ChangeState(EGameState.GeneratingMap);
            BoxModelManager.GenerateStream.OnEnd += _ => gameFsm.ChangeState(EGameState.Idle);
            
            BoxModelManager.DijkstraStream.Where(_ => isIdle);
            BoxModelManager.DijkstraStream.OnBegin += _ => gameFsm.ChangeState(EGameState.GeneratingMap);
            BoxModelManager.DijkstraStream.OnEnd += pair =>
            {
                gameFsm.ChangeState(EGameState.Playing);
                playerModel.OnEnterPlaying(pair.Item2);
            };
            
            gameFsm.ChangeState(EGameState.Idle);
            Binder.Update(gameFsm.Update);
        }
    }
}