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

        [field: MaybeNull] public static BindDataState IdleState =>
            field ??= Binder.From(gameFsm.GetState(EGameState.Idle));
        [field: MaybeNull] public static BindDataState GeneratingMapState => 
            field ??= Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        [field: MaybeNull] public static BindDataState PlayingState =>
            field ??= Binder.From(gameFsm.GetState(EGameState.Playing));
        [field: MaybeNull] public static BindDataState WatchingMapState =>
            field ??= Binder.From(gameFsm.GetState(EGameState.WatchingMap));
        
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
            BoxModelManager.DijkstraStream.OnEnd += pos3D =>
            {
                gameFsm.ChangeState(EGameState.Playing);
                // TODO
                // playerModel.OnEnterPlaying(pos3D);
            };
            
            Binder.Update(_ =>
            {
                if (Input.GetKeyDown(KeyCode.R))
                    BoxModelManager.GenerateStream.CallTriggerAsync();
            });
            
            gameFsm.ChangeState(EGameState.Idle);
            Binder.Update(gameFsm.Update);
        }
    }
}