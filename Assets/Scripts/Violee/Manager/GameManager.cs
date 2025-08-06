using System;
using System.Linq;
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

public enum EWindowType
{
    WaitingSceneItem,
    NormalUI,
    GamePause,
}

[Serializable]
public class WindowInfo
{
    public EWindowType WindowType;
    public string Des = string.Empty;
    [NonSerialized]
    public Action? OnAddEvent;
    [NonSerialized]
    public Action? OnRemoveEvent;
}

public static class WindowInfoExt
{
    public static WindowInfo OnAdd(this WindowInfo info, Action action)
    {
        info.OnAddEvent += action;
        return info;
    }
    public static WindowInfo OnRemove(this WindowInfo info, Action action)
    {
        info.OnRemoveEvent += action;
        return info;
    }
}


public class GameManager : SingletonCS<GameManager>
{
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static string GameState => gameFsm.CurStateName;
    public static readonly BindDataState GeneratingMapState;
    public static readonly BindDataState PlayingState;
    public static readonly MyList<WindowInfo> WindowList 
        = new ([], x => x.OnAddEvent?.Invoke(), x => x.OnRemoveEvent?.Invoke());
    public static readonly WindowInfo PauseWindow;

    bool init;
    public static void Init() => Instance.init = !Instance.init;

    static GameManager()
    {
        Configer.Init();
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        PlayingState
            .OnEnter(PlayerManager.OnEnterPlaying)
            .OnUpdate(dt =>
            {
                if (Input.GetKey(KeyCode.LeftAlt) || HasWindow)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                _ = MapManager.PlayerCurPointStream.CallTriggerAsync();
                PlayerManager.Tick(HasWindow);
            })
            .OnExit(PlayerManager.OnExitPlaying);
        
        PauseWindow = new WindowInfo()
        {
            WindowType = EWindowType.GamePause,
            Des = "游戏窗口失去焦点 or 按下了暂停键。",
            OnAddEvent = () =>
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            },
            OnRemoveEvent = () =>
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        };
        MapManager.GenerateStream
            .Where(_ => IsIdle || IsPlaying)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap));
        MapManager.DijkstraStream
            .OnEnd(param =>
            {
                PlayerManager.OnDijkstraEnd(param.PlayerStartPos);
                gameFsm.ChangeState(EGameState.Playing);
            });
            
        gameFsm.ChangeState(EGameState.Idle);
        Binder.Update(dt =>
        {
            gameFsm.Update(dt);
            CheckGameWindow();
        });
        Binder.Update(_ =>
        {
            Time.timeScale = Configer.SettingsConfig.QuickKey && Input.GetKey(KeyCode.Q) ? 10f : 1f;
            if (!IsIdle && !IsPlaying)
                return;
            if (Input.GetKeyDown(KeyCode.R))
            {
                Task.FromResult(MapManager.GenerateStream.CallTriggerAsync());
            }
        }, EUpdatePri.Input);
    }


    public static void UnPauseWindow()
     => WindowList.MyRemove(PauseWindow);
    static void CheckGameWindow()
    {
        if (!Application.isFocused)
        {
            if(!HasPaused)
                WindowList.MyAdd(PauseWindow);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!HasPaused)
                WindowList.MyAdd(PauseWindow);
            else
            {
                WindowList.MyRemove(PauseWindow);
            }
        }
    }

    
    public static bool IsIdle => gameFsm.IsState(EGameState.Idle);
    public static bool IsPlaying => gameFsm.IsState(EGameState.Playing);
    public static bool HasWindow => WindowList.Any();
    public static bool HasPaused => WindowList.Contains(PauseWindow);
}
