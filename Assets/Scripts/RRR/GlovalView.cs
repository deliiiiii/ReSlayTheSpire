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
        Binder.BindButton(errorPanel, () => errorPanel.SetActive(false));
        MainView.Bind();
        BattleView.Bind();
        MainView.gameObject.SetActive(true);
    }
    public void ShowError(string errorInfo)
    {
        errorPanel.SetActive(true);
        txtError.text = errorInfo;
    }
}
