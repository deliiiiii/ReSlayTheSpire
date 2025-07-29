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
                playerModel.OnEnterPlaying(pos3D);
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