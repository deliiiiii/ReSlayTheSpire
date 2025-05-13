using UnityEngine;
using UnityEngine.UI;


public class GlobalView : Singleton<GlobalView>
{
    
    [SerializeField] GameObject errorPanel;
    [SerializeField] Text txtError;

    protected override void OnInit()
    {
        base.OnInit();
        errorPanel.GetComponent<Button>().onClick.AddListener(()=>
        {
            errorPanel.SetActive(false);
        });
    }
    public void ShowError(string errorInfo)
    {
        errorPanel.SetActive(true);
        txtError.text = errorInfo;
    }
}