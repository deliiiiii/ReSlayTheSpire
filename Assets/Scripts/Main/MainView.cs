using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class MainView : ViewBase
{
    [HelpBox("Global",HelpBoxType.Info)]
    [SerializeField]
    public Text TxtPlayTime;
    [SerializeField] GameObject errorPanel;
    [SerializeField] Text txtError;
    
    
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
    public Text TxtJobName;

    public override void IBL()
    {
        gameObject.SetActive(true);
        RegisterModel(gameObject.GetOrAddComponent<MainModel>()).Init();
        Bind();
        MainModel.Launch();
    }
    
    
    void Bind()
    {
        Binder.From(errorPanel).To(() => errorPanel.SetActive(false));
        Binder.From(MainModel.PlayTime).ToTxt(TxtPlayTime).Immediate();
        Binder.From(MainModel.PlayerJob).ToTxt(TxtJobName).Immediate();
        Binder.From(btnStart).To(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        Binder.From(btnStart).To(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        Binder.From(btnQuit).To(() =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });
        Binder.From(btnSelectJobUp).To(MainModel.SetNextJob);
        Binder.From(btnSelectJobDown).To(MainModel.SetLastJob);
        Binder.From(btnConfirmJob).To(() =>
        {
            if (!MainModel.IsIronClad)
            {
                errorPanel.SetActive(true);
                txtError.text = "只有 铁甲战士 才能启动！";
                return;
            }
            MainModel.ChangeState(EMainState.Battle);
        });
        Binder.From(btnCancelJob).To(() =>
        {
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        });

        Binder.From(MainModel.GetState(EMainState.Title)).OnEnter(() =>
        {
            gameObject.SetActive(true);
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        });
        Binder.From(MainModel.GetState(EMainState.Title)).OnExit(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(false);
        });
        Binder.From(MainModel.GetState(EMainState.Battle)).OnEnter(() =>
        {
            BattleView.IBL();
        });
    }

    void Update()
    {
        MainModel.Tick(Time.deltaTime);
    }
}
