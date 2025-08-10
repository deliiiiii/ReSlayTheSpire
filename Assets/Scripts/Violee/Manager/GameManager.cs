using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public enum EGameState
{
    Title,
    GeneratingMap,
    Playing,
    Winning,
}

[Serializable]
public class WindowInfo
{
    public required string Des;
    [NonSerialized]
    public Action? OnAddEvent;
    [NonSerialized]
    public Action<WindowInfo>? OnAddEventWithArg;
    [NonSerialized]
    public Action? OnRemoveEvent;
    [NonSerialized]
    public Action<WindowInfo>? OnRemoveEventWithArg;
}

[Serializable]
public class PauseWindowInfo : WindowInfo
{
    public BindDataState TarState = null!;
}

[Serializable]
public class StringWindowInfo : WindowInfo
{
    public Func<string> GetWord = () => string.Empty;
}

[Serializable]
public class BuffWindowInfo : WindowInfo
{
    public GameObject? BuffWindowIns;
}

[Serializable]
public class ExchangeWindowInfo : WindowInfo
{
    public Action? OnExchangeEnd;
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
    public static WindowInfo OnRemoveWithArg(this WindowInfo info, Action<WindowInfo> action)
    {
        info.OnRemoveEventWithArg += action;
        return info;
    }
}


public class GameManager : SingletonCS<GameManager>
{
    bool init;
    
    
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static string GameState => gameFsm.CurStateName;
    public static readonly BindDataState TitleState;
    public static void EnterTitle() => gameFsm.ChangeState(EGameState.Title);
    public static readonly BindDataState WinningState;
    public static void EnterWinning() => gameFsm.ChangeState(EGameState.Winning);
    public static readonly BindDataState GeneratingMapState;
    public static readonly BindDataState PlayingState;
    
    
    public static readonly MyList<WindowInfo> WindowList
        = new ([], x =>
        {
            x.OnAddEvent?.Invoke();
            x.OnAddEventWithArg?.Invoke(x);
        }, x =>
        {
            x.OnRemoveEvent?.Invoke();
            x.OnRemoveEventWithArg?.Invoke(x);
        });
    public static readonly PauseWindowInfo PauseWindow = new ()
    {
        Des = "游戏窗口失去焦点 or 按下了暂停键。",
        OnAddEvent = EnableCursor,
        OnRemoveEvent = DisableCursor
    };
    public static readonly WindowInfo WatchingClockWindow = new ()
    {
        // WindowType = EWindowType.WaitingSceneItem,
        Des = "看时间...",
    };
    public static readonly StringWindowInfo WinWindow = new ()
    {
        Des = "Winning",
    };

    static void EnableCursor()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    static void DisableCursor()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    static BuffWindowInfo CreateAndAddBuffWindow(string des)
    {
        var ret = new BuffWindowInfo()
        {
            Des = des,
        };
        WindowList.MyAdd(ret);
        return ret;
    }


    static GameManager()
    {
        TitleState = Binder.From(gameFsm.GetState(EGameState.Title));
        TitleState
            .OnEnter(() =>
            {
                WindowList.MyClear();
                EnableCursor();
            })
            .OnUpdate(_ => PlayerMono.TickOnTitle());
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        PlayingState
            .OnEnter(PlayerMono.OnEnterPlaying)
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

                var curPoint = PlayerMono.PlayerCurPoint.Value;
                MapManager.GetPlayerVisit(PlayerMono.GetPos(), ref curPoint);
                PlayerMono.PlayerCurPoint.Value = curPoint;
                if(!HasPaused)
                    MapManager.OnPlaying(dt);
                PlayerMono.TickOnPlaying(HasWindow);
            })
            .OnExit(PlayerMono.OnExitPlaying);
        WinningState = Binder.From(gameFsm.GetState(EGameState.Winning));
        WinningState.OnEnter(() =>
        {
            WindowList.MyAdd(WinWindow);
        });
        
        MapManager.GenerateStream
            .Where(_ => IsTitle)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap));
        MapManager.DijkstraStream
            .OnEnd(param =>
            {
                PlayerMono.OnDijkstraEnd(param.PlayerStartPos);
                MainItemMono.OnDijkstraEnd();
                gameFsm.ChangeState(EGameState.Playing);
            });
        BuffManager.OnAddWindowBuff += winBuff =>
        {
            var ret = CreateAndAddBuffWindow($"{winBuff.Des}");
            ret.OnRemove(() => winBuff.BuffEffect());
        };
        Binder.Update(dt =>
        {
            gameFsm.Update(dt);
            CheckGameWindow();
        });
        Binder.Update(_ =>
        {
            Time.timeScale = Configer.SettingsConfig.QuickKey && Input.GetKey(KeyCode.Q) ? 10f : 1f;
        }, EUpdatePri.Input);
    }
    
    static void CheckGameWindow()
    {
        if (Configer.SettingsConfig.DisablePause)
            return;
        if (!IsPlaying)
            return;
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
                PauseWindow.TarState = PlayingState;
                WindowList.MyRemove(PauseWindow);
            }
        }
    }

    
    public static bool IsTitle => gameFsm.IsState(EGameState.Title);
    public static bool IsPlaying => gameFsm.IsState(EGameState.Playing);
    public static bool HasWindow => WindowList.Any();
    public static bool HasPaused => WindowList.Contains(PauseWindow);
}
