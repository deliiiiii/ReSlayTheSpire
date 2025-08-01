using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public enum EGameState
{
    Idle,
    GeneratingMap,
    Playing,
    Paused,
}

public class GameManager : Singleton<GameManager>
{
    [ShowInInspector]
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static BindDataState GeneratingMapState = null!;
    public static BindDataState PlayingState = null!;
    public static BindDataState PausedState = null!;
    
    protected override void Awake()
    {
        base.Awake();
        Configer.Init();
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        PausedState = Binder.From(gameFsm.GetState(EGameState.Paused));
        PlayingState.OnUpdate(dt =>
            {
                MapManager.TickPlayerVisit(PlayerManager.GetPos);
                PlayerManager.Tick(dt);
            })
            .OnEnter(PlayerManager.OnEnterPlaying)
            .OnExit(PlayerManager.OnExitPlaying);
            
        MapManager.GenerateStream
            .Where(_ => isIdle || isPlaying)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(_ => gameFsm.ChangeState(EGameState.Idle));

        MapManager.DijkstraStream
            .Where(_ => isIdle)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(pair =>
            {
                PlayerManager.OnDijkstraEnd(pair.Item2);
                gameFsm.ChangeState(EGameState.Playing);
            });
            
        gameFsm.ChangeState(EGameState.Idle);
        Binder.Update(gameFsm.Update);
        Binder.Update(_ => CheckGameWindow());
    }


    static EGameState stateBeforePause = EGameState.Idle;
    static void CheckGameWindow()
    {
        if (isPlaying && !Application.isFocused)
        {
            stateBeforePause = (EGameState)gameFsm.CurState;
            gameFsm.ChangeState(EGameState.Paused);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetKey(KeyCode.LeftAlt) || isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public static void ContinueFromPause()
    {
        gameFsm.ChangeState(stateBeforePause);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    static bool isIdle => gameFsm.IsState(EGameState.Idle);
    static bool isPlaying => gameFsm.IsState(EGameState.Playing);
    static bool isPaused => gameFsm.IsState(EGameState.Paused);
        
}