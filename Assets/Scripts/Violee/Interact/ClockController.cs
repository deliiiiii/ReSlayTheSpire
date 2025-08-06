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
    void OnEnable()
    {
        act ??= _ =>
        {
            if (GameManager.HasPaused && gameObject.activeSelf)
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
        // 获取当前时间
        DateTime now = MapManager.GetCurTime();
 
        // 计算角度（将时间转换为从12点开始的角度）
        float secondsAngle = (now.Second + now.Millisecond / 1000f) / 60f * 360f;
        float minutesAngle = (now.Minute + now.Second / 60f) / 60f * 360f;
        float hoursAngle = ((now.Hour % 12) + now.Minute / 60f) / 12f * 360f;
 
        // 更新手的位置（注意：这里直接设置了旋转，可能会导致“跳跃”的动画。如果需要平滑过渡，请使用Lerp等方法）
        secondHand.localRotation = Quaternion.Euler(0, 0, secondsAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, minutesAngle);
        hourHand.localRotation = Quaternion.Euler(0, 0, hoursAngle);
    }
}
