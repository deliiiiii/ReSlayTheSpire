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
        MyDebug.LogWarning($"GameManager Awake()");
        base.Awake();
        Configer.Init();
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
            
        PlayingState.OnUpdate(dt =>
        {
            MapManager.TickPlayerVisit(PlayerManager.GetPos);
            PlayerManager.Tick(dt);
        });
        PlayingState.OnExit(PlayerManager.OnExitPlaying);
        MyDebug.LogWarning($"GameManager Awake2()");
            
        MapManager.GenerateStream.Where(_ => isIdle || isPlaying);
        MapManager.GenerateStream.OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap));
        MapManager.GenerateStream.OnEnd(_ => gameFsm.ChangeState(EGameState.Idle));
            
        MapManager.DijkstraStream.Where(_ => isIdle);
        MapManager.DijkstraStream.OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap));
        MapManager.DijkstraStream.OnEnd(pair =>
        {
            PlayerManager.OnEnterPlaying(pair.Item2);
            gameFsm.ChangeState(EGameState.Playing);
        });
            
        gameFsm.ChangeState(EGameState.Idle);
        Binder.Update(gameFsm.Update);
        MyDebug.LogWarning($"GameManager Awake4()");
    }

    static bool isIdle => gameFsm.IsState(EGameState.Idle);
    static bool isPlaying => gameFsm.IsState(EGameState.Playing);
        
}