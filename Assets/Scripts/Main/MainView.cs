using UnityEngine;
using UnityEngine.UI;

public partial class MainView : MonoBehaviour
{
    [HelpBox("Global",HelpBoxType.Info)]
    [SerializeField]
    public Text TxtPlayTime;
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

    [SerializeField]
    Text txtJobName;
    
    public void Init()
    {
        Binder.BindText(MainModel.MainData.PlayTime, TxtPlayTime);
        
        btnStart.onClick.AddListener(()=>
        {
            panelTitle.SetActive(false);
            panelSelectJob.SetActive(true);
        });
        btnQuit.onClick.AddListener(()=>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
        errorPanel.GetComponent<Button>().onClick.AddListener(()=>
        {
            errorPanel.SetActive(false);
        });


        btnSelectJobUp.onClick.AddListener(()=>
        {
            MainModel.SetNextJob();
        });
        btnSelectJobDown.onClick.AddListener(()=>
        {
            MainModel.SetLastJob();
        });
        btnConfirmJob.onClick.AddListener(()=>
        {
            if(MainModel.PlayerJob != EJobType.IronClad)
            {
                MyEvent.Fire(new ErrorPanelEvent() { ErrorInfo = "只有 铁甲战士 才能启动！" });
                return;
            }
            MainModel.ChangeState(typeof(BattleState));
        });
        btnCancelJob.onClick.AddListener(()=>
        {
            panelTitle.SetActive(true);
            panelSelectJob.SetActive(false);
        });


        MyEvent.AddListener<OnEnterTitleStateEvent>(OnEnterTitleState);
        MyEvent.AddListener<OnPlayJobChangeEvent>(OnPlayJobChange);
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
    void OnPlayJobChange(OnPlayJobChangeEvent evt)
    {
        txtJobName.text = evt.PlayerJob.ToString();
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

