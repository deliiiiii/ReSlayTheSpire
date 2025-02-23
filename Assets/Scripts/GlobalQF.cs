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

public enum JobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}


public class GlobalModel : AbstractModel
{
    [Serializable]
    struct GlobalData
    {
        public string PlayerName;
        public float PlayTime;
        public StateData StateData;
    }
    GlobalData globalData;
    public float PlayTime
    {
        get
        {
            return globalData.PlayTime;
        }
        set
        {
            globalData.PlayTime = value;
            this.GetUtility<QJsonIO>().Save("Data","Global",globalData);
        }
    }
    protected override void OnInit()
    {
        GlobalData loadedData = this.GetUtility<QJsonIO>().Load<GlobalData>("Data","Global");
        if(loadedData.Equals(default))
        {
            PlayTime = 0f;
            globalData.PlayerName = "Deli_";
            globalData.StateData = new WaitForStartData()
            {
                StateType = typeof(WaitForStartState),
                subStateType = typeof(WaitForStartState_Title)
            };
        }
    }
}

public class GlobalSystem : AbstractSystem
{
    protected override void OnInit()
    {
        
    }
    public void Update()
    {
        GlobalArthitecture.Interface.GetModel<GlobalModel>().PlayTime += Time.deltaTime;
    }

}