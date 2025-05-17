using System;
using System.ComponentModel;
// using PropertyChanged;
using UnityEngine;



public enum EMainState
{
    Title,
    Battle,
}
public enum EJobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}
[Serializable] 
class MainData
{
    public string PlayerName;
    public Observable<float> PlayTime;
    public Observable<float> SaveTimer;
    public Observable<EJobType> PlayerJob;
}


public class MainModel : ModelBase
{
    [SerializeField]
    MyFSM<EMainState> mainFSM;
    [SerializeField]
    MainData mainData;
    public override void Init()
    {
        mainFSM = new MyFSM<EMainState>();
        mainData = Saver.Load<MainData>("Data",typeof(MainData).ToString());
        
        if(mainData == default(MainData) || mainData.PlayerName == null)
        {
            MyDebug.Log("mainData IS NULL");
            mainData = new MainData
            {
                PlayTime = new Observable<float>(0f),
                PlayerName = "Deli_"
            };
            Save("Init MainData");
        }
        Binder.From(mainData.SaveTimer).To(_ => Save("SaveTimer")).Culminate(5f);
    }

    public override void Launch()
    {
        ChangeState(EMainState.Title);
    }
    
    void Save(string info = "")
    {
        MyDebug.Log("Save " + info, LogType.State);
        Saver.Save("Data",typeof(MainData).ToString(), mainData);
    }
    
    public void Tick(float dt)
    {
        mainData.PlayTime.Value += dt;
        mainData.SaveTimer.Value += dt;
    }
    public void ChangeState(EMainState eState)
    {
        mainFSM.ChangeState(eState.ToString());
    }

    public bool IsIronClad => mainData.PlayerJob == EJobType.IronClad;
    public void SetNextJob()
    {
        SetJob((EJobType)(((int)PlayerJob.Value + 1) % Enum.GetValues(typeof(EJobType)).Length));
    }
    public void SetLastJob()
    {
        SetJob((EJobType)(((int)PlayerJob.Value - 1 + Enum.GetValues(typeof(EJobType)).Length) 
            % Enum.GetValues(typeof(EJobType)).Length));
    }
    void SetJob(EJobType eJobType)
    {
        mainData.PlayerJob.Value = eJobType;
        // Save("SetJob " + eJobType);
    }
    
    


    #region Getter
    public string PlayerName => mainData.PlayerName;
    public Observable<EJobType> PlayerJob => mainData.PlayerJob;
    public Observable<float> PlayTime => mainData.PlayTime;
    public MyState GetState(EMainState eState) => mainFSM.GetState(eState.ToString());
    #endregion
}