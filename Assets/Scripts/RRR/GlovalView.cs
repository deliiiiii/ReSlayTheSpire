using UnityEngine;
using UnityEngine.UI;


public class GlobalView : Singleton<GlobalView>
{
    
    [SerializeField] GameObject errorPanel;
    [SerializeField] Text txtError;

    [SerializeField] MainView mainView;
    public static MainView MainView => Instance.mainView;
    
    [SerializeField] BattleView battleView;
    public static BattleView BattleView => Instance.battleView;

    public override void OnInit()
    {
        base.OnInit();
        errorPanel.GetComponent<Button>().onClick.AddListener(()=>
        {
            errorPanel.SetActive(false);
        });
        
        
        MainView.Init();
        BattleView.Init();
        MainView.gameObject.SetActive(true);
    }
    public void ShowError(string errorInfo)
    {
        errorPanel.SetActive(true);
        txtError.text = errorInfo;
    }
}