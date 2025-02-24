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

        

        MyEvent.AddListener<OnStateChangeEvent>(OnStateChange);
        MyEvent.AddListener<OnPlayTimeChangeEvent>(OnPlayTimeChange);
    }

    
    void OnStateChange(OnStateChangeEvent evt)
    {
        panelTitle.SetActive(false);
        panelSelectJob.SetActive(false);
        if(evt.SubStateType == typeof(WaitForStartState_Title))
        {
            panelTitle.SetActive(true);
        }
        else if(evt.SubStateType == typeof(WaitForStartState_SelectJob))
        {
            panelSelectJob.SetActive(true);
        }
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
