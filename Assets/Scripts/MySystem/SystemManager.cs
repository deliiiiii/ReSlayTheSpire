using System;
using System.Collections.Generic; 
public static class SystemManager
{ 
    public static List<SystemBase> Systems = new();
    public static bool AddSystem(SystemBase system)
    {
        if (Systems.Contains(system))
        {
            MyDebug.LogError($"SystemManager 系统{system.GetType()}已存在");
            return false;
        }
        Systems.Add(system);
        return true;
    }
    public static void RemoveSystem(SystemBase system)
    {
        if (!Systems.Contains(system))
        {
            MyDebug.LogError($"SystemManager 系统{system.GetType()}不存在");
            return;
        }
        Systems.Remove(system);
    }
}
