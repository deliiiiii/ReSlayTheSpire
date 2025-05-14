using UnityEngine;
using UnityEngine.UI;


public interface IView
{
    public void Bind();
}

public class GlobalView : Singleton<GlobalView>, IView
{
    public Observable<float> ObFluent;
    public Text TxtFluent;
    
    [SerializeField] GameObject errorPanel;
    [SerializeField] Text txtError;

    [SerializeField] MainView mainView;
    public static MainView MainView => Instance.mainView;
    
    [SerializeField] BattleView battleView;
    public static BattleView BattleView => Instance.battleView;

    public override void OnInit()
    {
        // ObFluent = new Observable<float>(1);
        // TxtFluent.text = 0f.ToString("F3");
        // Binder.BindChangeFluent(ObFluent, TxtFluent, 0.5f, "F3");
        this.Bind();
        MainView.Bind();
        BattleView.Bind();
        MainView.gameObject.SetActive(true);
    }
    public void ShowError(string errorInfo)
    {
        errorPanel.SetActive(true);
        txtError.text = errorInfo;
    }

    public void Bind()
    {
        Binder.BindButton(errorPanel, () => errorPanel.SetActive(false));
    }
    
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Z))
    //     {
    //         ObFluent.Value += 1;
    //     }
    //     if (Input.GetKeyDown(KeyCode.X))
    //     {
    //         ObFluent.Value -= 1;
    //     }
    // }
}
