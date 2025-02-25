using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
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
    GameObject panelHandCard;
    float screenWidth;
    float handcardWidthFillPercent = 0.8f;
    float handcardHeightFillPercent = 0.9f;



    [SerializeField]
    UICard detailUICard;
    [SerializeField]
    UICard prefabUICard;




    public void Init()
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
        txtPlayerName.text = evt.PlayerName;
        txtJob.text = evt.PlayerData.Job.ToString();
        txtCurHP.text = evt.PlayerData.CurHP.ToString();
        txtMaxHP.text = evt.PlayerData.MaxHP.ToString();
        txtCurCoin.text = evt.PlayerData.Coin.ToString();
        Utils.ClearActiveChildren(panelKusuri.transform);
        foreach (var kusuri in evt.PlayerData.Kusuris)
        {
            // GameObject kusuriObj = Instantiate(panelKusuri, panelKusuri.transform);
            // kusuriObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Kusuri/" + kusuri.KusuriId);
        }
        txtDeckCardCount.text = evt.PlayerData.DeckCards.Count.ToString();
        Utils.ClearActiveChildren(transDeckCardContent);
        foreach (var cardData in evt.PlayerData.DeckCards)
        {
            GameObject go = CreateDeckCard(cardData);
        }
        ShowHandCard(evt.PlayerData.DeckCards);
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
        // uiCard.GetComponent<Canvas>().sortingOrder = (int)BattleSort.DeckCard;
        uiCard.ReadData(cardData);
        uiCard.GetComponent<Button>().enabled = true;
        uiCard.GetComponent<Button>().onClick.AddListener(()=>
        {
            MyEvent.Fire(new OnClickDeckCardEvent()
            {
                CardData = cardData,
            });
        });
        uiCard.transform.SetParent(transDeckCardContent);
        uiCard.gameObject.SetActive(true);
        return uiCard.gameObject;
    }

    GameObject CreateHandCard(CardData cardData)
    {
        UICard uiCard = Instantiate<UICard>(prefabUICard);
        // uiCard.GetComponent<Canvas>().sortingOrder = (int)BattleSort.HandCard;
        uiCard.ReadData(cardData);
        //TODO 
        uiCard.transform.SetParent(panelHandCard.transform);
        uiCard.gameObject.SetActive(true);
        return uiCard.gameObject;
    }


    void ShowHandCard(List<CardData> cardDatas)
    {
        float panelWidth = Screen.width * handcardWidthFillPercent;
        float cardHeight = panelHandCard.GetComponent<RectTransform>().sizeDelta.y * handcardHeightFillPercent;
        float cardScale = cardHeight / prefabUICard.GetComponent<RectTransform>().sizeDelta.y;
        float cardWidth = prefabUICard.GetComponent<RectTransform>().sizeDelta.x * cardScale;

        Utils.ClearActiveChildren(panelHandCard.transform);
        int cardCount = cardDatas.Count;
        if (cardCount == 0)
        {
            return;
        }
        if (cardCount == 1)
        {
            GameObject card = CreateHandCard(cardDatas[0]);
            card.transform.localScale = new Vector3(cardScale, cardScale, 1);
            card.transform.localPosition = Vector3.zero;
            return;
        }

        float spreadInterval = cardWidth * 0.1f;
        //计算所有手牌加间隙的最大宽度
        float spreadWidth = cardWidth * cardCount + spreadInterval * (cardCount - 1);
        if (spreadWidth > panelWidth)
        {
            float newInterval = (panelWidth - cardWidth * cardCount) / (cardCount - 1);
            spreadInterval = newInterval;
        }
        // 中心点是0，对应第centerID = （cardCount - 1）/ 2张牌，偏移量为 (curID - centerID)  * (cardWidth + spreadInterval)
        for (int i = 0; i < cardCount; i++)
        {
            float offset = (i - (cardCount - 1) / 2) * (cardWidth + spreadInterval);
            GameObject card = CreateHandCard(cardDatas[i]);
            card.transform.localScale = new Vector3(cardScale, cardScale, 1);
            card.transform.localPosition = new Vector3(offset, 0, 0);
        }
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

