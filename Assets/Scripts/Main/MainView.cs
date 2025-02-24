using UnityEngine;
using UnityEngine.UI;

public partial class MainView : Singleton<MainView>
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

    

    
    
    public void Init()
    {
        MyDebug.Log("MainView OnInit", LogType.State);
        InitMain();
        InitSelectJob();
        InitBattle();
        MyDebug.Log("MainView OnInit End", LogType.State);
    }

    void InitMain()
    {
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

        

        MyEvent.AddListener<OnEnterTitleStateEvent>(OnEnterTitleState);
        MyEvent.AddListener<OnEnterSelectJobStateEvent>(OnEnterSelectJobState);
        MyEvent.AddListener<OnPlayTimeChangeEvent>(OnPlayTimeChange);
    }

    void OnEnterTitleState(OnEnterTitleStateEvent evt)
    {
        panelTitle.SetActive(true);
        panelSelectJob.SetActive(false);
    }
    void OnEnterSelectJobState(OnEnterSelectJobStateEvent evt)
    {
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(true);
    }
    void OnPlayTimeChange(OnPlayTimeChangeEvent evt)
    {
        txtPlayTime.text = evt.PlayTime.ToString("F1");
    }

    public void ShowErrorPanel(string error)
    {
        errorPanel.SetActive(true);
        errorPanel.transform.Find("Txt Error").GetComponent<Text>().text = error;
    }
}
