using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Observable<float> ObFluent;
    public Text TxtFluent;
    
    [SerializeField] MainView mainView;
    public static MainView MainView => Instance.mainView;
    
    [SerializeField] BattleView battleView;
    public static BattleView BattleView => Instance.battleView;
    BindDataAct<float> b;
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
