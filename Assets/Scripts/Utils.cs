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
    public UpdateTimer(float time,Action action)
    {
        this.time = time;
        this.action = action;
    }
    public void Tick(float dt)
    {
        timer += dt;
        if(timer >= time)
        {
            action();
            timer -= time;
        }
    }
}

