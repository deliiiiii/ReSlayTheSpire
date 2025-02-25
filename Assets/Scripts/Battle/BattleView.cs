using Unity.VisualScripting;
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
    // [SerializeField]
    // Text txtBattleTime;
    // [SerializeField]
    // Button btnMap;
    [SerializeField]
    Button btnDeckCard;
    [SerializeField]
    Text txtDeckCardCount;
    [SerializeField]
    Button btnExit;
    
    [SerializeField]
    GameObject panelDeckCard;
    [SerializeField]
    GameObject panelDeckCardBack;
    [SerializeField]
    Transform transDeckCardContent;

    [SerializeField]
    GameObject panelDetailCard;

    [SerializeField]
    UICard detailUICard;
    [SerializeField]
    UICard prefabUICard;


    public void InitBattle()
    {
        MyEvent.AddListener<OnEnterBattleEvent>(OnEnterBattle);
        MyEvent.AddListener<OnClickDeckCardEvent>(OnClickDeckCard);
        MyEvent.AddListener<OnClickExitBattleEvent>(OnClickExitBattle);

        btnDeckCard.onClick.AddListener(()=>
        {
            panelDeckCard.SetActive(true);
        });
        panelDeckCardBack.GetComponent<Button>().onClick.AddListener(()=>
        {
            panelDeckCard.SetActive(false);
        });
        panelDetailCard.GetComponent<Button>().onClick.AddListener(()=>
        {
            panelDetailCard.SetActive(false);
        });
        btnExit.onClick.AddListener(()=>
        {
            MyEvent.Fire(new OnClickExitBattleEvent());
        });
        gameObject.SetActive(false);
    }

    void OnEnterBattle(OnEnterBattleEvent evt)
    {
        gameObject.SetActive(true);
        // txtPlayerName.text = evt.PlayerName;
        // txtJob.text = evt.Job;
        // txtCurHP.text = evt.CurHP.ToString();
        // txtMaxHP.text = evt.MaxHP.ToString();
        // txtCurCoin.text = evt.Coin.ToString();
        txtPlayerName.text = MainModel.PlayerName;
        txtJob.text = MainModel.SelectJobModel.SelectedJob.ToString();
        txtCurHP.text = evt.PlayerData.CurHP.ToString();
        txtMaxHP.text = evt.PlayerData.MaxHP.ToString();
        txtCurCoin.text = evt.PlayerData.Coin.ToString();
        Utils.ClearActiveChildren(panelKusuri.transform);
        foreach (var kusuri in evt.PlayerData.kusuris)
        {
            // GameObject kusuriObj = Instantiate(panelKusuri, panelKusuri.transform);
            // kusuriObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Kusuri/" + kusuri.KusuriId);
        }
        txtDeckCardCount.text = evt.PlayerData.deckCards.Count.ToString();
        Utils.ClearActiveChildren(transDeckCardContent);
        foreach (var cardData in evt.PlayerData.deckCards)
        {
            GameObject go = CreateDeckCard(cardData);
            go.transform.SetParent(transDeckCardContent);
        }
    }

    void OnClickDeckCard(OnClickDeckCardEvent evt)
    {
        detailUICard.ReadData(evt.CardData);
        panelDetailCard.SetActive(true);
    }

    void OnClickExitBattle(OnClickExitBattleEvent evt)
    {
        gameObject.SetActive(false);
        MyCommand.Send(new OnConfirmExitBattleCommand());
    }

    GameObject CreateDeckCard(CardData cardData)
    {
        UICard uiCard = Instantiate<UICard>(prefabUICard);
        uiCard.ReadData(cardData);
        uiCard.GetComponent<Button>().onClick.AddListener(()=>
        {
            MyEvent.Fire(new OnClickDeckCardEvent()
            {
                CardData = cardData,
            });
        });
        uiCard.gameObject.SetActive(true);
        return uiCard.gameObject;
    }
}


public class OnClickDeckCardEvent
{
    public CardData CardData;
}

public class OnClickExitBattleEvent
{
}

public class OnExitBattleEvent
{
}

