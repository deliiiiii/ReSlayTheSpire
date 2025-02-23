using System;
using QFramework;
using UnityEngine;

public class GlobalModel : AbstractModel
{
    class GlobalData
    {
        public string PlayerName;
        public float PlayTime;
        public string StateName;
        public string SubStateName;
    }
    GlobalData globalData = new();
    public float PlayTime
    {
        get
        {
            return globalData.PlayTime;
        }
    }

    MyFSM globalFSM = new();
    protected override void OnInit()
    {   
        
    }

    public void InitAfterController()
    {
        MyDebug.Log("GlobalModel OnInit", LogType.State);   
        GlobalData loadedData = this.GetUtility<QJsonIO>().Load<GlobalData>("Data","Global");
        MyDebug.Log("loadedData: " + (loadedData == null));
        if(loadedData == null)
        {
            globalData.PlayTime = 0f;
            globalData.PlayerName = "Deli_";
            globalData.StateName = typeof(WaitForStartState).ToString();
            globalData.SubStateName = typeof(WaitForStartState_Title).ToString();
        }
        else
        {
            globalData = loadedData;
        }
        SetStateWithoutSave(Type.GetType(globalData.StateName), Type.GetType(globalData.SubStateName));
        // MyDebug.Log("PlayTime: " + PlayTime, LogType.State);
    }
    void SetStateWithoutSave(Type stateType, Type subStateType)
    {
        globalFSM.ChangeState(stateType, subStateType);
        this.SendEvent(new OnStateChangeEvent()
        {
            stateType = stateType,
            subStateType = subStateType
        });
    }
    public void SetState(Type stateType, Type subStateType)
    {
        globalData.StateName = stateType.ToString();
        globalData.SubStateName = subStateType.ToString();
        Save();
        SetStateWithoutSave(stateType, subStateType);
    }
    public void Save()
    {
        MyDebug.Log("Save", LogType.State);
        this.GetUtility<QJsonIO>().Save("Data", "Global", globalData);
    }
    public void Update(float dt)
    {
        globalData.PlayTime += dt;
        globalFSM.Update();
    }
     
}

public class GlobalSystem : AbstractSystem
{
    private float saveTimer = 0f;
    
    public void Update(float dt)
    {
        this.GetModel<GlobalModel>().Update(dt);
        
        saveTimer += dt;
        if(saveTimer >= 5f)
        {
            saveTimer = 0f;
            this.GetModel<GlobalModel>().Save();
        }
    }

    protected override void OnInit()
    {
        
    }
}


public struct OnStateChangeEvent
{
    public Type stateType;
    public Type subStateType;

}