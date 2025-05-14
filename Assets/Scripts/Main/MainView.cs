using UnityEngine;
using UnityEngine.UI;

public partial class MainView : MonoBehaviour
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
    
    public void Init()
    {
        Binder.BindChange(MainModel.PlayTime, TxtPlayTime, true);
        Binder.BindChange(MainModel.PlayerJob, TxtJobName, true);
        btnStart.onClick.AddListener(() =>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        btnQuit.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
        
        btnSelectJobUp.onClick.AddListener(MainModel.SetNextJob);
        btnSelectJobDown.onClick.AddListener(MainModel.SetLastJob);
        btnConfirmJob.onClick.AddListener(()=>
        {
            if(MainModel.PlayerJob != EJobType.IronClad)
            {
                GlobalView.Instance.ShowError("只有 铁甲战士 才能启动！");
                return;
            }
            MainModel.ChangeState(typeof(BattleState));
        });
        btnCancelJob.onClick.AddListener(()=>
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
