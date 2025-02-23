using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour, IController
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
    void Awake()
    {
        btnStart.onClick.AddListener(()=>
        {
            this.SendCommand<OnClickStartCommand>();
        });
        btnQuit.onClick.AddListener(()=>
        {
            this.SendCommand<OnClickQuitCommand>();
        });

        btnConfirmJob.onClick.AddListener(()=>
        {
            this.SendCommand<OnClickConfirmJobCommand>();
        });
        btnCancelJob.onClick.AddListener(()=>
        {
            this.SendCommand<OnClickCancelJobCommand>();
        });

        this.RegisterEvent<OnStateChangeEvent>(OnStateChange);
        
        this.GetModel<GlobalModel>().InitAfterController();
    }
    void Update()
    {
        this.GetModel<GlobalModel>().Update(Time.deltaTime);
        // 保留1位小数
        txtPlayTime.text = this.GetModel<GlobalModel>().PlayTime.ToString("F1");
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

    public IArchitecture GetArchitecture()
    {
        return GlobalArthitecture.Interface;
    }
}
