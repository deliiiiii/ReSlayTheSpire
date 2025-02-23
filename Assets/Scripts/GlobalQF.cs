using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
public class GlobalArthitecture : Architecture<GlobalArthitecture>
{
    protected override void Init()
    {
        this.RegisterModel(new GlobalModel());
        this.RegisterUtility(new QJsonIO());
        this.RegisterSystem(new GlobalSystem());
    }
}

public class QJsonIO : IUtility
{
    public void Save<T>(string f_pathPre,string f_name,T curEntity)
    {
        JsonIO.Write(f_pathPre,f_name,curEntity);
    }
    public T Load<T>(string f_pathPre,string f_name)
    {
        return JsonIO.Read<T>(f_pathPre,f_name);
    }
    public void Delete(string f_pathPre,string f_name)
    {
        JsonIO.Delete(f_pathPre,f_name);
    }
}
