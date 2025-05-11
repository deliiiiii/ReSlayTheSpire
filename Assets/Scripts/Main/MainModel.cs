using System;
using System.ComponentModel;
using PropertyChanged;
using UnityEngine;
public enum EJobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}


[Serializable] 
[AddINotifyPropertyChangedInterface]
public partial class MainData
{
    public string PlayerName;
    public float PlayTime {get; set;}
    void OnPlayerNameChanged()
    {
        MyEvent.Fire(new OnPlayTimeChangeEvent()
        {
            PlayTime = PlayTime
        });
    }
    
    public string StateName;
    public EJobType PlayerJob {get; set;}
    void OnPlayerJobChanged()
    {
        MyEvent.Fire(new OnPlayJobChangeEvent()
        {
            PlayerJob = PlayerJob
        });
    }
}

public class MainModel
{
    public static MyFSM mainFSM;
    static MainData mainData;
    public static void Init()
    {
        mainFSM = new();
        mainData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        MyDebug.Log("mainData IS NULL : " + (mainData == default(MainData)));
        if(mainData == default(MainData) || mainData.PlayerName == null)
        {
            mainData = new()
            {
                PlayTime = 0f,
                PlayerName = "Deli_",
                StateName = typeof(WaitForStartState_Title).ToString(),
            };
            Save("Init MainData");
        }
        // string stateName = mainData.StateName;
        // mainFSM.ChangeState(Type.GetType(stateName));
        ChangeState(typeof(WaitForStartState_Title));
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
    public static void ChangeState(Type stateType)
    {
        // mainData.StateName = stateType.ToString();
        mainFSM.ChangeState(stateType);
        // Save("SetState" + mainData.StateName);
    }


    public static void SetNextJob()
    {
        SetJob((EJobType)(((int)PlayerJob + 1) % Enum.GetValues(typeof(EJobType)).Length));
    }
    public static void SetLastJob()
    {
        SetJob((EJobType)(((int)PlayerJob - 1 + Enum.GetValues(typeof(EJobType)).Length) 
            % Enum.GetValues(typeof(EJobType)).Length));
    }
    static void SetJob(EJobType eJobType)
    {
        mainData.PlayerJob = eJobType;
        Save("SetJob " + eJobType.ToString());
    }



    #region Getter
    public static string PlayerName => mainData.PlayerName;
    public static EJobType PlayerJob => mainData.PlayerJob;
    #endregion
}


public class OnPlayTimeChangeEvent
{
    public float PlayTime;
}

public class OnPlayJobChangeEvent
{
    public EJobType PlayerJob;
}
