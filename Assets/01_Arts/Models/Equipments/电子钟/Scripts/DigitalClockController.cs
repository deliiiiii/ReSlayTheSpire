using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

public class DigitalClockController : MonoBehaviour
{
    public TextMeshProUGUI textHour;
    public TextMeshProUGUI textMinute;
    public TextMeshProUGUI textSecond;
    public TextMeshProUGUI textYear;
    public TextMeshProUGUI textMonth;
    public TextMeshProUGUI textDay;
    public TextMeshProUGUI textDayOfWeek;

    private static readonly Dictionary<string, string> dayOfWeekMapping = new Dictionary<string, string>
    {
        { "星期一", "1" },
        { "星期二", "2" },
        { "星期三", "3" },
        { "星期四", "4" },
        { "星期五", "5" },
        { "星期六", "6" },
        { "星期日", "8" }
    };

    void Start()
    {
        if (textHour == null || textMinute == null || textSecond == null || textYear == null || textMonth == null || textDay == null || textDayOfWeek == null)
        {
            Debug.LogError("One or more TextMeshProUGUI components are missing in the DigitalClockController.");
            return;
        }
        // 开始每秒更新时钟
        InvokeRepeating("UpdateClock", 0f, 1f);
    }

    void UpdateClock()
    {
        DateTime now = DateTime.Now;

        // 分别获取时、分、秒并赋值给对应的Text组件
        string hourString = now.ToString("HH");
        string minuteString = now.ToString("mm");
        string secondString = now.ToString("ss");

        string yearString = now.ToString("yyyy");
        string monthString = now.ToString("MM"); // 月份，两位数字
        string dayString = now.ToString("dd");   // 日期，两位数字
        string dayOfWeekString = now.ToString("dddd");

        // 将星期几文字转换为对应的数字
        if (dayOfWeekMapping.TryGetValue(dayOfWeekString, out string dayOfWeekNumber))
        {
            textDayOfWeek.text = dayOfWeekNumber;
        }
        else
        {
            Debug.LogWarning($"Unknown day of week: {dayOfWeekString}");
            textDayOfWeek.text = "?"; // 或者其他默认值
        }

        textHour.text = hourString;
        textMinute.text = minuteString;
        textSecond.text = secondString;
        textYear.text = yearString;
        textMonth.text = monthString;
        textDay.text = dayString;
    }

    void OnDisable()
    {
        // 当GameObject被禁用时，取消所有InvokeRepeating调用
        CancelInvoke();
    }

    void OnDestroy()
    {
        // 当GameObject被销毁时，取消所有InvokeRepeating调用
        CancelInvoke();
    }
}