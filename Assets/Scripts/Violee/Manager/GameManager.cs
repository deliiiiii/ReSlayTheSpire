using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public enum EGameState
{
    Idle,
    GeneratingMap,
    Playing,
}

public enum EWindowState
{
    None,
    Paused,
    WatchingUI,
}

public class GameManager : SingletonCS<GameManager>
{
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static string GameState => gameFsm.CurStateName;
    public static readonly BindDataState GeneratingMapState;
    public static readonly BindDataState PlayingState;
    static readonly MyFSM<EWindowState> windowFsm = new ();
    public static string WindowState => windowFsm.CurStateName;
    public static readonly BindDataState NoneState;
    public static readonly BindDataState PausedState;

    public static void Init() => Instance.As();

    static GameManager()
    {
        Configer.Init();
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        PlayingState.OnUpdate(dt =>
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                MapManager.TickPlayerVisit(PlayerManager.GetPos);
                PlayerManager.Tick(dt);
            })
            .OnEnter(PlayerManager.OnEnterPlaying)
            .OnExit(PlayerManager.OnExitPlaying);
        
        NoneState = Binder.From(windowFsm.GetState(EWindowState.None));
        PausedState = Binder.From(windowFsm.GetState(EWindowState.Paused));
        NoneState.OnEnter(() =>
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }).OnUpdate(_ => CheckGameWindow());
        PausedState.OnEnter(() =>
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        });
        
        MapManager.GenerateStream
            .Where(_ => isIdle || isPlaying)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(_ => gameFsm.ChangeState(EGameState.Idle));
        MapManager.DijkstraStream
            .Where(_ => isIdle)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap))
            .OnEnd(param =>
            {
                PlayerManager.OnDijkstraEnd(param.PlayerStartPos);
                gameFsm.ChangeState(EGameState.Playing);
            });
            
        gameFsm.ChangeState(EGameState.Idle);
        windowFsm.ChangeState(EWindowState.None);
        
        Binder.Update(dt =>
        {
            if (isPaused)
                return;
            gameFsm.Update(dt);
            windowFsm.Update(dt);
        });
        Binder.Update(_ =>
        {
            if (!isIdle && !isPlaying)
                return;
            if (Input.GetKeyDown(KeyCode.R))
                Task.FromResult(MapManager.GenerateStream.CallTriggerAsync());
        }, EUpdatePri.Input);
    }
    
    
    public static void UnpauseWindow() => windowFsm.ChangeState(EWindowState.None);

    public static void SwitchUIState()
    {
        windowFsm.ChangeState(!IsWatchingUI ? EWindowState.WatchingUI : EWindowState.None);
    }
    
    static void CheckGameWindow()
    {
#pragma warning disable CS0162 // 检测到不可到达的代码
#if UNITY_EDITOR
        return;
#endif
        if (!Application.isFocused || Input.GetKeyDown(KeyCode.Escape))
        {
            windowFsm.ChangeState(EWindowState.Paused);
        }
#pragma warning restore CS0162
    }
    public static bool isIdle => gameFsm.IsState(EGameState.Idle);
    public static bool isPlaying => gameFsm.IsState(EGameState.Playing);
    public static bool isPaused => windowFsm.IsState(EWindowState.Paused);
    public static bool IsWatchingUI => windowFsm.IsState(EWindowState.WatchingUI);
        
}