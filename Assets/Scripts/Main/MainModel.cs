using System;
using UnityEngine;

[Serializable]
public partial class MainData
{
    public string PlayerName;
    [SerializeField]
    float playTime;
    public float PlayTime
    {
        get
        {
            return playTime;
        }
        set
        {
            playTime = value;
            MyEvent.Fire(new OnPlayTimeChangeEvent()
            {
                PlayTime = value
            });
        }
    }
    public string StateName;
    [SerializeField]
    string subStateName;
    public string SubStateName
    {
        get
        {
            return subStateName;
        }
        set
        {
            subStateName = value;
        }
    }
}

public partial class MainModel
{
    public static MyFSM mainFSM;
    static MainData mainData;
    public static void Init()
    {
        mainFSM = new();
        MyDebug.Log("MainModel OnInit", LogType.State);   
        mainData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        MyDebug.Log("loadedData IS NULL : " + (mainData == default(MainData)));
        if(mainData == default(MainData) || mainData.PlayerName == null)
        {
            mainData = new()
            {
                PlayTime = 0f,
                PlayerName = "Deli_",
                SubStateName = typeof(WaitForStartState_Title).ToString(),
                StateName = typeof(WaitForStartState).ToString(),
            };
            Save("Init MainData");
        }
        string stateName = mainData.StateName;
        string subStateName = mainData.SubStateName;
        if(! string.IsNullOrEmpty(subStateName))
        {
            mainFSM.ChangeState(Type.GetType(stateName), Type.GetType(subStateName));
        }
        else
        {
            mainFSM.ChangeState(Type.GetType(stateName));
        }
        // MyDebug.Log("MainModel OnInit End", LogType.State);
    }
    
    public static void Save(string info = "")
    {
        MyDebug.Log("Save " + info, LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), mainData);
    }
    public static void Tick(float dt)
    {
        mainData.PlayTime += dt;
    }
    public static void SetState(Type stateType, Type subStateType)
    {
        mainData.SubStateName = subStateType?.ToString();
        mainData.StateName = stateType.ToString();
        mainFSM.ChangeState(stateType, subStateType);
        Save("SetState" + mainData.StateName + " " + mainData.SubStateName);
    }
}


public class OnPlayTimeChangeEvent
{
    public float PlayTime;
}

