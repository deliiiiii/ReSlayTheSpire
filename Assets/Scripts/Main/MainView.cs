using UnityEngine;
using UnityEngine.UI;

public partial class MainView : MonoBehaviour
{
    [HelpBox("Global",HelpBoxType.Info)]
    [SerializeField]
    Text txtPlayTime;
    [SerializeField]
    GameObject errorPanel;
    
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

    EJobType m_EJobType;
    EJobType JobType
    {
        get { return m_EJobType; }
        set { m_EJobType = value; txtJobName.text = value.ToString(); }
    }

    [SerializeField]
    Text txtJobName;
    
    public void Init()
    {
        btnStart.onClick.AddListener(()=>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
            JobType = EJobType.IronClad;
        });
        btnQuit.onClick.AddListener(()=>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            UnityEngine.Application.Quit();
            #endif
        });
        errorPanel.GetComponent<Button>().onClick.AddListener(()=>
        {
            errorPanel.SetActive(false);
        });


        btnSelectJobUp.onClick.AddListener(()=>
        {
            //TODO Model in UI calss
            JobType = MainModel.GetNextJob(JobType);
        });
        btnSelectJobDown.onClick.AddListener(()=>
        {
            //TODO Model in UI calss
            JobType = MainModel.GetLastJob(JobType);
        });
        btnConfirmJob.onClick.AddListener(()=>
        {
            //TODO Unexpected logic in UI class
            if(JobType != EJobType.IronClad)
            {
                MyEvent.Fire(new ErrorPanelEvent() { ErrorInfo = "只有 铁甲战士 才能启动！" });
                return;
            }
            //TODO Model in UI calss
            MainModel.SetJob(JobType);
            MainModel.ChangeState(typeof(BattleState));
        });
        btnCancelJob.onClick.AddListener(()=>
        {
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        });


        MyEvent.AddListener<OnEnterTitleStateEvent>(OnEnterTitleState);
        MyEvent.AddListener<OnPlayTimeChangeEvent>(OnPlayTimeChange);
        MyEvent.AddListener<OnEnterBattleEvent>(OnEnterBattleState);
        MyEvent.AddListener<ErrorPanelEvent>(OnShowErrorPanel);
    }



    #region Event
    void OnEnterTitleState(OnEnterTitleStateEvent evt)
    {
        gameObject.SetActive(true);
        panelTitle.SetActive(true);
        panelSelectJob.SetActive(false);
    }

    void OnPlayTimeChange(OnPlayTimeChangeEvent evt)
    {
        txtPlayTime.text = evt.PlayTime.ToString("F1");
    }
    
    void OnEnterBattleState(OnEnterBattleEvent evt)
    {
        panelSelectJob.SetActive(false);
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(false);
    }

    void OnShowErrorPanel(ErrorPanelEvent evt)
    {
        errorPanel.SetActive(true);
        errorPanel.transform.Find("Txt Error").GetComponent<Text>().text = evt.ErrorInfo;
    }
    #endregion
}


public class ErrorPanelEvent
{
    public string ErrorInfo;
}

