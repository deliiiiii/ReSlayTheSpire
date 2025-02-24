using UnityEngine;
using UnityEngine.UI;

public class BattleView : Singleton<BattleView>
{
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
    GameObject panelKusuri;
    [SerializeField]
    Text txtBattleTime;
    [SerializeField]
    Button btnMap;
    [SerializeField]
    Button btnDeckCard;
    [SerializeField]
    Text txtDeckCardCount;
    [SerializeField]
    Button btnExit;
    
    [SerializeField]
    GameObject panelDeckCard;


    public void InitBattle()
    {
        MyEvent.AddListener<OnEnterBattleEvent>(OnEnterBattle);

        btnDeckCard.onClick.AddListener(()=>
        {
            panelDeckCard.SetActive(true);
        });
        panelDeckCard.GetComponent<Button>().onClick.AddListener(()=>
        {
            panelDeckCard.SetActive(false);
        });
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
