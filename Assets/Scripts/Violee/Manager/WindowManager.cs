using System;
using System.Linq;
using UnityEngine;

namespace Violee;

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

public class WindowManager : Singleton<WindowManager>
{
    
    static WindowManager()
    {
        GameManager.TitleState
            .OnEnter(() =>
            {
                WindowList.MyClear();
                EnableCursor();
            });

        GameManager.PlayingState
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
            });

        GameManager.WinningState
            .OnEnter(() =>
            {
                WindowList.MyAdd(WinWindow);
            });
        
        BuffManager.OnAddWindowBuff += winBuff =>
        {
            var ret = CreateAndAddBuffWindow($"{winBuff.Des}");
            ret.OnRemove(() => winBuff.BuffEffect());
        };
        
        Binder.Update(_ => CheckGameWindow(), EUpdatePri.Input);
    }
    
    
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
    
    static void CheckGameWindow()
    {
        if (Configer.SettingsConfig.DisablePause)
            return;
        if (!GameManager.IsPlaying)
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
                PauseWindow.TarState = GameManager.PlayingState;
                WindowList.MyRemove(PauseWindow);
            }
        }
    }
    public static bool HasWindow => WindowList.Any();
    public static bool HasPaused => WindowList.Contains(PauseWindow);
    public static bool HasWatchingClock => WindowList.Contains(WatchingClockWindow);
}