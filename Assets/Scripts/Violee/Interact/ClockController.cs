using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Violee;

public class ClockController : MonoBehaviour
{
    public required Transform hourHand;
    public required Transform minuteHand;
    public required Transform secondHand;

    Action<float>? act;
    readonly Observable<DateTime> curTime = new(default);

    void Awake()
    {
        curTime.OnValueChangedFull += (oldTime, newTime) =>
        {
            if (GameManager.WindowList.Contains(GameManager.WatchingClockWindow)
                && PlayerManager.InteractInfo.Value is SceneItemInteractInfo itemInfo
                && itemInfo.SceneItemData == GetComponent<SceneItemModel>().Data
                && newTime.Hour != oldTime.Hour
               )
            {
                BuffManager.WindowWatchingOClock(newTime.Hour);
            }
        };
    }

    void OnEnable()
    {
        act ??= _ =>
        {
            if (GameManager.HasPaused)
                return;
            Tick();
        };
        GameManager.PlayingState.OnUpdate(act);
    }

    void OnDisable()
    {
        GameManager.PlayingState.RemoveUpdate(act);
    }

    public void Tick()
    {
        var now = curTime.Value = MapManager.GetCurTime();
 
        float secondsAngle = (now.Second + now.Millisecond / 1000f) / 60f * 360f;
        float minutesAngle = (now.Minute + now.Second / 60f) / 60f * 360f;
        float hoursAngle = ((now.Hour % 12) + now.Minute / 60f) / 12f * 360f;
        
        secondHand.localRotation = Quaternion.Euler(0, 0, secondsAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, minutesAngle);
        hourHand.localRotation = Quaternion.Euler(0, 0, hoursAngle);
    }
}
