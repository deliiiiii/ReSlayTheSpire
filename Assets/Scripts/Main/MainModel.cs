using System;

[Serializable]
public partial class MainData
{
    public string PlayerName;
    public float PlayTime;
    public string StateName;
    public string SubStateName;
    public MainData()
    {
        PlayTime = 0f;
        PlayerName = "Deli_";
        StateName = typeof(WaitForStartState).ToString();
        SubStateName = typeof(WaitForStartState_Title).ToString();
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
        if(loadedData == null)
        {
            mainData = new();
        }
        else
        {
            mainData = loadedData;
        }
        SetStateWithoutSave(Type.GetType(mainData.StateName), Type.GetType(mainData.SubStateName));
    }

    


    static void SetStateWithoutSave(Type stateType, Type subStateType)
    {
        MyEvent.Fire(new OnStateChangeEvent()
        {
            stateType = stateType,
            subStateType = subStateType
        });
    }



    
    public static void Save()
    {
        MyDebug.Log("Save", LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), mainData);
    }
    public static void Tick(float dt)
    {
        mainData.PlayTime += dt;
        MyEvent.Fire(new OnPlayTimeChangeEvent()
        {
            playTime = mainData.PlayTime
        });
    }


    public static void SetState(Type stateType, Type subStateType)
    {
        mainData.StateName = stateType.ToString();
        mainData.SubStateName = subStateType.ToString();
        Save();
        SetStateWithoutSave(stateType, subStateType);
    }


    
    
}


public struct OnStateChangeEvent
{
    public Type stateType;
    public Type subStateType;
}

public struct OnPlayTimeChangeEvent
{
    public float playTime;
}