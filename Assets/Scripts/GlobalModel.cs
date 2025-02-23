using System;
using QFramework;
using UnityEngine;

public class GlobalModel : AbstractModel
{
    class GlobalData
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
    }
    public StateData StateData
    {
        get
        {
            return globalData.StateData;
        }
    }
    protected override void OnInit()
    {
        MyDebug.Log("GlobalModel OnInit", LogType.State);   
        GlobalData loadedData = this.GetUtility<QJsonIO>().Load<GlobalData>("Data","Global");
        if(loadedData.Equals(default(GlobalData)))
        {
            globalData.PlayTime = 0f;
            globalData.PlayerName = "Deli_";
            globalData.StateData = new StateData()
            {
                StateType = typeof(WaitForStartState).ToString(),
                subStateType = typeof(WaitForStartState_Title).ToString()
            };
            return;
        }
        globalData = loadedData;
        MyDebug.Log("PlayTime: " + PlayTime, LogType.State);
    }
    public void Save()
    {
        this.GetUtility<QJsonIO>().Save("Data", "Global", globalData);
    }
    public void Tick(float dt)
    {
        globalData.PlayTime += dt;
    }
}

public class GlobalSystem : AbstractSystem
{
    private float saveTimer = 0f;
    MyFSM globalFSM;
    protected override void OnInit()
    {
        // MyDebug.Log("GlobalSystem OnInit", LogType.State);
        Type stateType = Type.GetType(this.GetModel<GlobalModel>().StateData.StateType);
        Type subStateType = Type.GetType(this.GetModel<GlobalModel>().StateData.subStateType);
        // MyDebug.Log("stateType: " +this.GetModel<GlobalModel>().StateData.StateType + " " + stateType.ToString(), LogType.State);
        // MyDebug.Log("subStateType: " +this.GetModel<GlobalModel>().StateData.subStateType + " " + subStateType.ToString(), LogType.State);
        globalFSM = new(stateType, subStateType);
    }
    public void ChangeState(Type stateType, Type subStateType = null)
    {
        globalFSM.ChangeState(stateType, subStateType);
        this.SendEvent(new OnStateChangeEvent()
        {
            stateType = stateType,
            subStateType = subStateType
        });

    }
    public void Update()
    {
        this.GetModel<GlobalModel>().Tick(Time.deltaTime);
        globalFSM.Update();
        
        saveTimer += Time.deltaTime;
        if(saveTimer >= 5f)
        {
            MyDebug.Log("Save" + this.GetModel<GlobalModel>().PlayTime,LogType.State);
            saveTimer = 0f;
            this.GetModel<GlobalModel>().Save();
        }
    }
}


public struct OnStateChangeEvent
{
    public Type stateType;
    public Type subStateType;

}