using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;


public class Saver
{
    public static void Save<T>(string f_pathPre,string f_name,T curEntity)
    {
        JsonIO.Write(f_pathPre,f_name,curEntity);
    }
    public static T Load<T>(string f_pathPre,string f_name)
    {
        return JsonIO.Read<T>(f_pathPre,f_name);
    }
    public static void Delete(string f_pathPre,string f_name)
    {
        JsonIO.Delete(f_pathPre,f_name);
    }
}
public class UpdateTimer
{
    float time;
    float timer;
    Action action;
    bool stopped = false;
    public UpdateTimer(float time,Action action)
    {
        this.time = time;
        this.action = action;
    }
    public void Tick(float dt)
    {
        if(stopped)
            return;
        timer += dt;
        if(timer >= time)
        {
            action();
            timer -= time;
        }
    }
    public void Stop()
    {
        stopped = true;
    }
    public void Resume()
    {
        stopped = false;
    }
    public void ResetAndResume()
    {
        timer = 0;
        stopped = false;
    }
}

public class Utils
{
    public static void ClearActiveChildren(Transform trans)
    {
        for(int i = 0; i < trans.childCount; i++)
        {
            if (!trans.GetChild(i).gameObject.activeSelf)
                continue;
            GameObject.Destroy(trans.GetChild(i).gameObject);
        }
    }
}