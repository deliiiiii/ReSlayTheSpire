using System;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    [HelpBox("Global",HelpBoxType.Info)]
    [SerializeField]
    public Text TxtPlayTime;
    
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


    

    public void Bind()
    {
        Binder.BindChange(MainModel.PlayTime, TxtPlayTime);
        Binder.BindChange(MainModel.PlayerJob, TxtJobName);
        Binder.BindButton(btnStart, () =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        Binder.BindButton(btnQuit, () =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });
        Binder.BindButton(btnSelectJobUp, MainModel.SetNextJob);
        Binder.BindButton(btnSelectJobDown, MainModel.SetLastJob);
        Binder.BindButton(btnConfirmJob, () =>
        {
            if(MainModel.PlayerJob != EJobType.IronClad)
            {
                GlobalView.Instance.ShowError("只有 铁甲战士 才能启动！");
                return;
            }
            MainModel.ChangeState(typeof(BattleState));
        });
        Binder.BindButton(btnCancelJob, () =>
        {
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        });
    }



    #region Event
    public void OnEnterTitleState()
    {
        gameObject.SetActive(true);
        panelTitle.SetActive(true);
        panelSelectJob.SetActive(false);
    }
    
    public void OnEnterBattleState()
    {
        panelSelectJob.SetActive(false);
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(false);
    }

    
    #endregion
}
