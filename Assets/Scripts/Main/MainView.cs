using System;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
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

    public void IBL()
    {
        MainModel.Init();
        Bind();
        MainModel.Launch();
    }
    
    
    void Bind()
    {
        Binder.From(errorPanel).SingleTo(() => errorPanel.SetActive(false));
        Binder.From(MainModel.PlayTime).ToTxt(TxtPlayTime);
        Binder.From(MainModel.PlayerJob).ToTxt(TxtJobName);
        Binder.From(btnStart).SingleTo(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        Binder.From(btnStart).SingleTo(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        Binder.From(btnQuit).SingleTo(() =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });
        Binder.From(btnSelectJobUp).SingleTo(MainModel.SetNextJob);
        Binder.From(btnSelectJobDown).SingleTo(MainModel.SetLastJob);
        Binder.From(btnConfirmJob).SingleTo(() =>
        {
            if (!MainModel.IsIronClad)
            {
                errorPanel.SetActive(true);
                txtError.text = "只有 铁甲战士 才能启动！";
                return;
            }
            MainModel.ChangeState(EMainState.Battle);
        });
        Binder.From(btnCancelJob).SingleTo(() =>
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
            UIManager.BattleView.IBL();
        });
    }
}
