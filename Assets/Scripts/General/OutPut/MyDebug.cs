using System.Collections.Generic;
using UnityEngine;
public enum LogType
{
    Default,
    Card,
    State,
    Drag,
    BattleUI,
}
public static class MyDebug
{
    //设置输出严重度，输出类型，是否输出
    static bool canLogAll = true;
    static bool canLog = true;
    static bool canLogWarning = true;
    static bool canLogError = true;
    static int logThreshold = 10;
    static List<LogType> logTypes = new()
    {
        LogType.Default,
        // LogType.Card,
        // LogType.State,
        LogType.Drag,
        LogType.BattleUI,
    };
    public static void Log(object message, LogType logType = LogType.Default, int threshold = 0)
    {
        if (!canLog || !CheckLog(logType, threshold))
        {
            return;
        }
        Debug.Log(message);
        
    }

    public static void LogWarning(object message, LogType logType = LogType.Default, int threshold = 0)
    {
        if (!canLogWarning || !CheckLog(logType, threshold))
        {
            return;
        }
        Debug.LogWarning(message);

    }
    public static void LogError(object message, LogType logType = LogType.Default, int threshold = 0)
    {
        if (!canLogError || !CheckLog(logType, threshold))
        {
            return;
        }
        Debug.LogError(message);

    }

    static bool CheckLog( LogType logType, int threshold)
    {
        if (!canLogAll || logThreshold < threshold || !logTypes.Contains(logType))
        {
            return false;
        }
        return true;
    }
}