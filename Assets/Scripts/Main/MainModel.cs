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
public class MainData
{
    public string PlayerName;
    public Observable<float> PlayTime;
    public Observable<float> SaveTimer;
    public Observable<EJobType> PlayerJob;
}

public static class MainModel
{
    static MyFSM mainFSM;
    static MainData mainData;
    public static void Init()
    {
        mainFSM = new MyFSM();
        mainData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        MyDebug.Log("mainData IS NULL : " + (mainData == default(MainData)));
        if(mainData == default(MainData) || mainData.PlayerName == null)
        {
            mainData = new MainData
            {
                PlayTime = new Observable<float>(0f),
                PlayerName = "Deli_"
            };
            Save("Init MainData");
        }

        Binder.BindCulminate(mainData.SaveTimer, 5f, () => Save("SaveTimer"));
    }
        
    
    
    public static void Save(string info = "")
    {
        MyDebug.Log("Save " + info, LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), mainData);
    }
    public static void Tick(float dt)
    {
        mainData.PlayTime.Value += dt;
        mainData.SaveTimer.Value += dt;
    }
    public static void ChangeState(Type stateType)
    {
        mainFSM.ChangeState(stateType);
    }


    public static void SetNextJob()
    {
        SetJob((EJobType)(((int)PlayerJob.Value + 1) % Enum.GetValues(typeof(EJobType)).Length));
    }
    public static void SetLastJob()
    {
        SetJob((EJobType)(((int)PlayerJob.Value - 1 + Enum.GetValues(typeof(EJobType)).Length) 
            % Enum.GetValues(typeof(EJobType)).Length));
    }
    static void SetJob(EJobType eJobType)
    {
        mainData.PlayerJob.Value = eJobType;
        // Save("SetJob " + eJobType);
    }



    #region Getter
    public static string PlayerName => mainData.PlayerName;
    public static Observable<EJobType> PlayerJob => mainData.PlayerJob;
    public static Observable<float> PlayTime => mainData.PlayTime;

    #endregion
}