using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

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
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static BindDataState GeneratingMapState = null!;
    public static BindDataState PlayingState = null!;
    protected override void Awake()
    {
        base.Awake();
        Configer.Init();
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
            
        PlayingState.OnUpdate(dt =>
            {
                MapManager.TickPlayerVisit(PlayerManager.GetPos);
                PlayerManager.Tick(dt);
            }).OnExit(PlayerManager.OnExitPlaying);
            
        MapManager.GenerateStream
            .Where(_ => isIdle || isPlaying)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(_ => gameFsm.ChangeState(EGameState.Idle));
            
        MapManager.DijkstraStream
            .Where(_ => isIdle)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(pair =>
            {
                PlayerManager.OnEnterPlaying(pair.Item2);
                gameFsm.ChangeState(EGameState.Playing);
            });
            
        gameFsm.ChangeState(EGameState.Idle);
        Binder.Update(gameFsm.Update);
    }

    static bool isIdle => gameFsm.IsState(EGameState.Idle);
    static bool isPlaying => gameFsm.IsState(EGameState.Playing);
        
}