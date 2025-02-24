using UnityEngine;
using UnityEngine.UI;

public partial class MainView : Singleton<MainView>
{
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

    public void InitSelectJob()
    {
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

        MyEvent.AddListener<OnSelectedJobChangeEvent>(OnSelectedJobChange);
    }

    void OnSelectedJobChange(OnSelectedJobChangeEvent evt)
    {
        txtJobName.text = evt.JobType.ToString();
    }
}