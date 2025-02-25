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
                StateName = typeof(WaitForStartState_Title).ToString(),
            };
            Save("Init MainData");
        }
        string stateName = mainData.StateName;
        mainFSM.ChangeState(Type.GetType(stateName));
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
    public static void SetState(Type stateType)
    {
        mainData.StateName = stateType.ToString();
        mainFSM.ChangeState(stateType);
        Save("SetState" + mainData.StateName);
    }

    #region Getter
    public static string PlayerName => mainData.PlayerName;
    #endregion
}


public class OnPlayTimeChangeEvent
{
    public float PlayTime;
}

