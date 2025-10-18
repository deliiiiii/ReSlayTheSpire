using System;
using System.Linq;
using UnityEngine;

namespace Violee;

public interface IMyListItem
{
    public Action? OnAddEvent { get; set; }
    public Action? OnRemoveEvent{ get; set; }

    public Action? CallOnAddEventWithArg{ get; }
    public Action? CallOnRemoveEventWithArg{ get; }
}
public interface IMyListItem<T> : IMyListItem
{
    Action? IMyListItem.CallOnAddEventWithArg => OnAddEventWithArg == null ? null : () => OnAddEventWithArg((T)this);
    Action? IMyListItem.CallOnRemoveEventWithArg => OnRemoveEventWithArg == null ? null : () => OnRemoveEventWithArg((T)this);
    public Action<T>? OnAddEventWithArg { get; set; }
    public Action<T>? OnRemoveEventWithArg { get; set; }

}

[Serializable]
public abstract class WindowInfo<T> : IMyListItem<T> where T : WindowInfo<T>
{
    public required string Des;
    public Action<T>? OnAddEventWithArg { get; set; }
    public Action<T>? OnRemoveEventWithArg { get; set; }
    
    public Action? OnAddEvent { get; set; }
    public Action? OnRemoveEvent{ get; set; }
}

public class SimpleWindowInfo : WindowInfo<SimpleWindowInfo>;


[Serializable]
public class PauseWindowInfo : WindowInfo<PauseWindowInfo>
{
    public StateHolder TarState = null!;
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


public static class MyListExt
{
    public static T OnAdd<T>(this T self, Action action)
        where T : IMyListItem<T>
    {
        self.OnAddEvent += action;
        return self;
    }
    public static T OnRemove<T>(this T self, Action action)
        where T : IMyListItem<T>
    {
        self.OnRemoveEvent += action;
        return self;
    }

    public static T OnAddWithArg<T>(this T self, Action<T> action)
        where T : IMyListItem<T>
    {
        self.OnAddEventWithArg += action;
        return self;
    }
    public static T OnRemoveWithArg<T>(this T self, Action<T> action)
        where T : IMyListItem<T>
    {
        self.OnRemoveEventWithArg += action;
        return self;
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

    public static readonly MyList<IMyListItem> WindowList = new([]);
    // TODO AddListener
        // = new ([], x =>
        // {
        //     x.OnAddEvent?.Invoke();
        //     x.CallOnAddEventWithArg?.Invoke();
        // }, x =>
        // {
        //     x.OnRemoveEvent?.Invoke();
        //     x.CallOnRemoveEventWithArg?.Invoke();
        // });
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