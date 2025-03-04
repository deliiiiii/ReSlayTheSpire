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
    [SerializeField]
    Text txtJobName;
    
    public void Init()
    {
        MyEvent.RegisterAnnotatedHandlers(this);
        btnStart.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickStartCommand());
        });
        btnQuit.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickQuitCommand());
        });
        errorPanel.GetComponent<Button>().onClick.AddListener(()=>
        {
            errorPanel.SetActive(false);
        });


        btnSelectJobUp.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickNextJobCommand());
        });
        btnSelectJobDown.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickLastJobCommand());
        });
        btnConfirmJob.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickConfirmJobCommand());
        });
        btnCancelJob.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnClickCancelJobCommand());
        });


        // MyEvent.AddListener<OnEnterTitleStateEvent>(OnEnterTitleState);
        // MyEvent.AddListener<OnEnterSelectJobStateEvent>(OnEnterSelectJobState);
        // MyEvent.AddListener<OnPlayTimeChangeEvent>(OnPlayTimeChange);
        // MyEvent.AddListener<OnEnterBattleEvent>(OnEnterBattleState);
        // MyEvent.AddListener<ErrorPanelEvent>(OnShowErrorPanel);
        // MyEvent.AddListener<OnSelectedJobChangeEvent>(OnSelectedJobChange);
    }

    [MyEvent]
    void OnSelectedJobChange(OnSelectedJobChangeEvent evt)
    {
        txtJobName.text = evt.JobType.ToString();
    }

    [MyEvent]
    void OnEnterTitleState(OnEnterTitleStateEvent evt)
    {
        gameObject.SetActive(true);
        panelTitle.SetActive(true);
        panelSelectJob.SetActive(false);
    }

    [MyEvent]
    void OnPlayTimeChange(OnPlayTimeChangeEvent evt)
    {
        txtPlayTime.text = evt.PlayTime.ToString("F1");
    }

    [MyEvent]
    void OnEnterSelectJobState(OnEnterSelectJobStateEvent evt)
    {
        gameObject.SetActive(true);
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(true);
        txtJobName.text = evt.JobType.ToString();
    }
    
    [MyEvent]
    void OnEnterBattleState(OnEnterBattleEvent evt)
    {
        panelSelectJob.SetActive(false);
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(false);
    }

   [MyEvent]
    void OnShowErrorPanel(ErrorPanelEvent evt)
    {
        errorPanel.SetActive(true);
        errorPanel.transform.Find("Txt Error").GetComponent<Text>().text = evt.ErrorInfo;
    }
}


public class ErrorPanelEvent
{
    public string ErrorInfo;
}

