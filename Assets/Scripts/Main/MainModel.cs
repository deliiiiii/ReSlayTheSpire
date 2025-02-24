using System;
using UnityEngine;
public interface IData<T>
{
    public void ReadData(T savedData);
}

[Serializable]
public partial class MainData : IData<MainData>
{
    [SerializeField]
    string playerName;
    public string PlayerName
    {
        get => playerName; 
        set
        {
            playerName = value;
        }
    }
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
    [SerializeField]
    string stateName;
    public string StateName
    {
        get
        {
            return stateName;
        }
        set
        {
            stateName = value;
            MyEvent.Fire(new OnStateChangeEvent()
            {
                SubStateType = Type.GetType(subStateName),
                StateType = Type.GetType(stateName),
            });
        }
    }
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


    public MainData()
    {
        PlayTime = 0f;
        PlayerName = "Deli_";
        SubStateName = typeof(WaitForStartState_Title).ToString();
        StateName = typeof(WaitForStartState).ToString();
    }
    public void ReadData(MainData savedData)
    {
        PlayTime = savedData.PlayTime;
        PlayerName = savedData.PlayerName;
        SubStateName = savedData.SubStateName;
        StateName = savedData.StateName;
        SelectJobData.ReadData(savedData.SelectJobData);
    }
}

public partial class MainModel
{
    static MainData mainData = new();
    
    public static void Init()
    {
        MyDebug.Log("MainModel OnInit", LogType.State);   
        MainData loadedData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        MyDebug.Log("loadedData:（IS NULL :） " + (loadedData == null));
        if(loadedData != null)
        {
            mainData.ReadData(loadedData);
        }
        MyDebug.Log("MainModel OnInit End", LogType.State);
    }
    
    public static void Save()
    {
        MyDebug.Log("Save", LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), mainData);
    }
    public static void Tick(float dt)
    {
        mainData.PlayTime += dt;
    }
    public static void SetState(Type stateType, Type subStateType)
    {
        mainData.SubStateName = subStateType.ToString();
        mainData.StateName = stateType.ToString();
        Save();
    }
}


public class OnStateChangeEvent
{
    public Type SubStateType;
    public Type StateType;
}

public class OnPlayTimeChangeEvent
{
    public float PlayTime;
}

