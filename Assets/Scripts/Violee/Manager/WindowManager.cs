using System;
using System.Linq;
using UnityEngine;

namespace Violee;

public interface IMyListItem
{
    public Action? OnAddEvent { get;}
    public Action? OnRemoveEvent{ get;}
    public Action? CallOnAddEventWithArg{ get; }
    public Action? CallOnRemoveEventWithArg{ get; }
    
}

[Serializable]
public abstract class WindowInfo<T> : IMyListItem where T : WindowInfo<T>
{
    public required string Des;
   

    public Action<T>? OnAddEventWithArg;
    public Action<T>? OnRemoveEventWithArg;
    
    public Action? OnAddEvent { get; set; }
    public Action? OnRemoveEvent{ get; set; }
    public Action CallOnAddEventWithArg => () => OnAddEventWithArg?.Invoke((this as T)!);

    public Action CallOnRemoveEventWithArg => () => OnRemoveEventWithArg?.Invoke((this as T)!);
}

public class SimpleWindowInfo : WindowInfo<SimpleWindowInfo>;


[Serializable]
public class PauseWindowInfo : WindowInfo<PauseWindowInfo>
{
    public BindDataState TarState = null!;
}

[Serializable]
public class StringWindowInfo : WindowInfo<StringWindowInfo>
{
    public Func<string> GetWord = () => string.Empty;
}

[Serializable]
public class BuffWindowInfo : WindowInfo<BuffWindowInfo>
{
    public GameObject? BuffWindowIns;
}

[Serializable]
public class ExchangeWindowInfo : WindowInfo<ExchangeWindowInfo>
{
    public Action? OnExchangeEnd;
}


public static class WindowInfoExt
{
    public static WindowInfo<T> OnAdd<T>(this WindowInfo<T> info, Action action)
        where T : WindowInfo<T>
    {
        info.OnAddEvent += action;
        return info;
    }
    public static WindowInfo<T> OnRemove<T>(this WindowInfo<T> info, Action action)
        where T : WindowInfo<T>
    {
        info.OnRemoveEvent += action;
        return info;
    }

    public static WindowInfo<T> OnAddWithArg<T>(this WindowInfo<T> info, Action<T> action)
        where T : WindowInfo<T>
    {
        info.OnAddEventWithArg += action;
        return info;
    }
    public static WindowInfo<T> OnRemoveWithArg<T>(this WindowInfo<T> info, Action<T> action)
        where T : WindowInfo<T>
    {
        info.OnRemoveEventWithArg += action;
        return info;
    }
}

public class WindowManager : Singleton<WindowManager>
{
    
    static WindowManager()
    {
        GameState.TitleState
            .OnEnter(() =>
            {
                WindowList.MyClear();
                EnableCursor();
            });

        GameState.PlayingState
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

                CheckGameWindow();
            });

        GameState.WinningState
            .OnEnter(() =>
            {
                WindowList.MyAdd(WinWindow);
            });
        
        BuffManager.OnAddWindowBuff += winBuff =>
        {
            var ret = CreateAndAddBuffWindow($"{winBuff.Des}");
            ret.OnRemove(() => winBuff.BuffEffect());
        };
    }
    
    
    public static readonly MyList<IMyListItem> WindowList
        = new ([], x =>
        {
            x.OnAddEvent?.Invoke();
            x.CallOnAddEventWithArg?.Invoke();
        }, x =>
        {
            x.OnRemoveEvent?.Invoke();
            x.CallOnRemoveEventWithArg?.Invoke();
        });
    public static readonly PauseWindowInfo PauseWindow = new ()
    {
        Des = "游戏窗口失去焦点 or 按下了暂停键。",
        OnAddEvent = EnableCursor,
        OnRemoveEvent = DisableCursor
    };
    public static readonly SimpleWindowInfo WatchingClockWindow = new ()
    {
        // WindowType = EWindowType.WaitingSceneItem,
        Des = "看时间...",
    };
    public static readonly StringWindowInfo WinWindow = new ()
    {
        Des = "Winning",
    };
    
    public static readonly SimpleWindowInfo FullMapWindow = new ()
    {
        Des = "全屏地图"
    };
    public static readonly SimpleWindowInfo SleepWindow = new ()
    {
        Des = "休息..."
    };
    public static readonly SimpleWindowInfo DrawWindow = new ()
    {
        Des = "选择房间装修中"
    };
    public static readonly StringWindowInfo VioleTWindow = new ()
    {
        Des = "VioleT",
    };

    public static readonly SimpleWindowInfo DicWindow = new()
    {
        Des = "查看单词表",
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
                PauseWindow.TarState = GameState.PlayingState;
                WindowList.MyRemove(PauseWindow);
            }
        }
    }
    public static bool HasWindow => WindowList.Any();
    public static bool HasPaused => WindowList.Contains(PauseWindow);
    public static bool HasWatchingClock => WindowList.Contains(WatchingClockWindow);
}