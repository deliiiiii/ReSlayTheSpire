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
    public Observable<float> PlayTime;
    public float SaveTimer;
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
    public static MyFSM MainFSM;
    public static MainData MainData;
    public static void Init()
    {
        MainFSM = new();
        MainData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        MyDebug.Log("mainData IS NULL : " + (MainData == default(MainData)));
        if(MainData == default(MainData) || MainData.PlayerName == null)
        {
            MainData = new MainData
            {
                PlayTime = new Observable<float>(0f),
                PlayerName = "Deli_",
                StateName = typeof(WaitForStartState_Title).ToString(),
            };
            Save("Init MainData");
        }
        
        Binder.BindAction(MainData.PlayTime,
            () =>
            {
                MainData.SaveTimer += Time.deltaTime;
                if (MainData.SaveTimer >= 5f)
                {
                    MainData.SaveTimer -= 5f;
                    Save("SaveTimer");
                }
            }
                
                );
    }
    
    public static void Save(string info = "")
    {
        MyDebug.Log("Save " + info, LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), MainData);
    }
    public static void Tick(float dt)
    {
        MainData.PlayTime.Value += dt;
    }
    public static void ChangeState(Type stateType)
    {
        MainFSM.ChangeState(stateType);
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
        MainData.PlayerJob = eJobType;
        Save("SetJob " + eJobType.ToString());
    }



    #region Getter
    public static string PlayerName => MainData.PlayerName;
    public static EJobType PlayerJob => MainData.PlayerJob;

    // public static Observable<float> PlayTime
    // {
    //     get => mainData.PlayTime;
    //     set => mainData.PlayTime = value;
    // }

    #endregion
}


public class OnPlayJobChangeEvent
{
    public EJobType PlayerJob;
}
