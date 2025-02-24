using UnityEngine;
using UnityEngine.UI;

public partial class MainView : Singleton<MainView>
{
    [HelpBox("Battle",HelpBoxType.Info)]
    [SerializeField]
    Text txtPlayerName;
    [SerializeField]
    Text txtJob;
    [SerializeField]
    Text txtCurHP;
    [SerializeField]
    Text txtMaxHP;
    [SerializeField]
    Text txtCurCoin;
    [SerializeField]
    GameObject panelKurusi;
    [SerializeField]
    Text txtBattleTime;
    [SerializeField]
    Button btnMap;
    [SerializeField]
    Button btnExit;
    [SerializeField]
    Button btnHandCard;


    void InitBattle()
    {
        MyEvent.AddListener<OnEnterBattleEvent>(OnEnterBattle);
    }

    void OnEnterBattle(OnEnterBattleEvent evt)
    {
        // txtPlayerName.text = evt.PlayerName;
        // txtJob.text = evt.Job;
        // txtCurHP.text = evt.CurHP.ToString();
        // txtMaxHP.text = evt.MaxHP.ToString();
        // txtCurCoin.text = evt.Coin.ToString();
    }
}
