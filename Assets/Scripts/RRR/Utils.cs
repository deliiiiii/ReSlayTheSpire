using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;


public static class Saver
{
    public static void Save<T>(string pathPre, string name, T curEntity)
    {
        JsonIO.Write(pathPre,name,curEntity);
    }
    public static T Load<T>(string pathPre,string name)
    {
        return JsonIO.Read<T>(pathPre,name);
    }
    public static void Delete(string pathPre,string name)
    {
        JsonIO.Delete(pathPre,name);
    }
}

public static class Utils
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