using UnityEngine;
using UnityEngine.UI;

public class GlobalView : Singleton<GlobalView>
{
    public Observable<float> ObFluent;
    public Text TxtFluent;
    
    [SerializeField] MainView mainView;
    public static MainView MainView => Instance.mainView;
    
    [SerializeField] BattleView battleView;
    public static BattleView BattleView => Instance.battleView;
    BindDataAct<float> b;
    public override void OnInit()
    {
        MainView.Bind();
        BattleView.Bind();
        MainView.gameObject.SetActive(true);
        
        
        ObFluent = new Observable<float>(1);
        TxtFluent.text = 0f.ToString("F3");
        b = Binder.From(ObFluent).ToTxt(TxtFluent).Fluent(.5f).Format("F3").Immediate();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ObFluent.Value += 1;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ObFluent.Value -= 1;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            b.UnBind();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            b.ReBind().Immediate();
        }
    }
}
