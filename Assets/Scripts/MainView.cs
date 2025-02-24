using System;
using UnityEngine;
using UnityEngine.UI;

public class MainView : Singleton<MainView>
{
    #region WaitForStart
    [HelpBox("WaitForStart",HelpBoxType.Info)]
    [Header("Title")]
    [SerializeField]
    GameObject panelTitle; 
    [SerializeField]
    Button btnStart;
    [SerializeField]
    Button btnQuit;

    [Header("SelectJob")]
    [SerializeField]
    GameObject panelSelectJob;
    [SerializeField]
    Button btnSelectJobUp;
    [SerializeField]
    Button btnSelectJobDown;
    [SerializeField]
    Button btnConfirmJob;
    [SerializeField]
    Button btnCancelJob;
    #endregion

    
    [HelpBox("Global",HelpBoxType.Info)]
    [SerializeField]
    Text txtPlayTime;
    public void Init()
    {
        btnStart.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickStartCommand());
        });
        btnQuit.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickQuitCommand());
        });

        btnConfirmJob.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickConfirmJobCommand());
        });
        btnCancelJob.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickCancelJobCommand());
        });

        MyEvent.AddListener<OnStateChangeEvent>(OnStateChange);
        MyEvent.AddListener<OnPlayTimeChangeEvent>(OnPlayTimeChange);
    }

    void OnStateChange(OnStateChangeEvent evt)
    {
        if(evt.subStateType == typeof(WaitForStartState_Title))
        {
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        }
        else if(evt.subStateType == typeof(WaitForStartState_SelectJob))
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        }
    }
    void OnPlayTimeChange(OnPlayTimeChangeEvent evt)
    {
        txtPlayTime.text = evt.playTime.ToString("F1");
    }

}
