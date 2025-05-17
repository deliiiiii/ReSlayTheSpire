using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable InconsistentNaming

public class BattleView : MonoBehaviour
{
    [SerializeField]UICard prefabUICard;


    [Header("Enter Next Room")]
    [SerializeField]GameObject panelEnterNextRoom;
    [SerializeField]UIMapNodeEnemy uiMapNodeEnemy;

    [Header("Hand Card")]
    [SerializeField]GameObject panelHandCard;

    const float handcardWidthFillPercent = 0.8f;
    const float handcardHeightFillPercent = 0.9f;


    [Header("Deck Card")]
    [SerializeField]GameObject panelDeckCard;
    [SerializeField]GameObject panelDeckCardBack;
    [SerializeField]Transform transDeckCardContent;


    [Header("Detail Card")]
    [SerializeField]GameObject panelDetailCard;
    [SerializeField]UICard detailUICard;


    [Header("Upper Info")]
    [SerializeField]Text txtPlayerName;
    [SerializeField]Text txtJob;
    [SerializeField]Text txtCurHP;
    [SerializeField]Text txtMaxHP;
    [SerializeField]Text txtCurCoin;
    [SerializeField]GameObject panelKusuri;
    // [SerializeField]Text txtBattleTime;
    // [SerializeField]Button btnMap;
    [SerializeField]Button btnDeckCard;
    [SerializeField]Text txtDeckCardCount;
    [SerializeField]Button btnExit;
    
    public void Bind()
    {
        Binder.From(btnDeckCard).SingleTo(() => panelDeckCard.SetActive(true));
        Binder.From(panelDeckCardBack).SingleTo(() => panelDeckCard.SetActive(false));
        Binder.From(panelDetailCard).SingleTo(() => panelDetailCard.SetActive(false));
        Binder.From(btnExit).SingleTo(() =>
        {
            gameObject.SetActive(false);
            MainModel.ChangeState(EMainState.Title);
        });
        Binder.From(BattleModel.GetState(EBattleState.InRoom)).OnEnter(OnEnterBattle);
        
        uiMapNodeEnemy.Bind();
    }


    public void OnEnterBattle()
    {
        MyDebug.Log("UI OnEnterGlobalBattle", LogType.BattleUI);
        gameObject.SetActive(true);
        txtPlayerName.text = MainModel.PlayerName;
        txtJob.text = MainModel.PlayerJob.ToString();
        txtCurHP.text = BattleModel.PlayerData.CurHP.ToString();
        txtMaxHP.text = BattleModel.PlayerData.MaxHP.ToString();
        txtCurCoin.text = BattleModel.PlayerData.Coin.ToString();
        Utils.ClearActiveChildren(panelKusuri.transform);
        // foreach (var kusuri in evt.PlayerData.Kusuris)
        // {
        //     GameObject kusuriObj = Instantiate(panelKusuri, panelKusuri.transform);
        //     kusuriObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Kusuri/" + kusuri.KusuriId);
        // }
        txtDeckCardCount.text = BattleModel.PlayerData.DeckCards.Count.ToString();
        Utils.ClearActiveChildren(transDeckCardContent);
        foreach (var cardData in BattleModel.PlayerData.DeckCards)
        {
            CreateDeckCard(cardData);
        }        
        ShowHandCard(BattleModel.PlayerData.DeckCards);
    }

    // void OnEnterSelectNextRoom(OnEnterSelectNextRoomStateEvent evt)
    // {
    //     MyDebug.Log("UI OnEnterSelectNextRoom", LogType.BattleUI);
    //     panelHandCard.SetActive(false);
    //     panelDeckCard.SetActive(false);
    //     panelDetailCard.SetActive(false);
    //     panelEnterNextRoom.SetActive(true);
    //     uiMapNodeEnemy.SetEnemyType(new []{"Enemy1", "Enemy2", "Enemy3"});
    //     uiMapNodeEnemy.SetCurSelectedEnemyType(evt.EnemyType);
    // }

    public void EnterInRoomBattle()
    {
        MyDebug.Log("UI OnEnterInRoomBattle", LogType.BattleUI);
        panelEnterNextRoom.SetActive(false);
        panelHandCard.SetActive(true);
        panelDeckCard.SetActive(false);
        panelDetailCard.SetActive(false);
    }

    GameObject CreateDeckCard(CardData cardData)
    {
        UICard uiCard = Instantiate(prefabUICard, transDeckCardContent);
        uiCard.ReadData(cardData);
        uiCard.GetComponent<Button>().enabled = true;
        uiCard.GetComponent<Button>().onClick.AddListener(()=>
        {
            detailUICard.ReadData(cardData);
            panelDetailCard.SetActive(true);
        });
        uiCard.gameObject.SetActive(true);
        return uiCard.gameObject;
    }

    GameObject CreateHandCard(CardData cardData)
    {
        UICard uiCard = Instantiate(prefabUICard, panelHandCard.transform);
        uiCard.IsHandCard = true;
        uiCard.ReadData(cardData);
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
        switch (cardCount)
        {
            case 0:
                return;
            case 1:
            {
                GameObject card = CreateHandCard(cardDatas[0]);
                card.transform.localScale = new Vector3(cardScale, cardScale, 1);
                card.transform.localPosition = Vector3.zero;
                return;
            }
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
            float offset = (i - (cardCount - 1f) / 2) * (cardWidth + spreadInterval);
            GameObject card = CreateHandCard(cardDatas[i]);
            card.transform.localScale = new Vector3(cardScale, cardScale, 1);
            card.transform.localPosition = new Vector3(offset, 0, 0);
        }
    }
    
    float lastScreenWidth;
    void RefreshHandCardPos()
    {
        if (Math.Abs(lastScreenWidth - Screen.width) < 1f)
        {
            return;
        }
        lastScreenWidth = Screen.width;
        float panelWidth = Screen.width * handcardWidthFillPercent;
        float cardHeight = panelHandCard.GetComponent<RectTransform>().sizeDelta.y * handcardHeightFillPercent;
        float cardScale = cardHeight / prefabUICard.GetComponent<RectTransform>().sizeDelta.y;
        float cardWidth = prefabUICard.GetComponent<RectTransform>().sizeDelta.x * cardScale;

        int cardCount = panelHandCard.transform.childCount;
        float spreadInterval = cardWidth * 0.1f;
        float spreadWidth = cardWidth * cardCount + spreadInterval * (cardCount - 1);
        if (spreadWidth > panelWidth)
        {
            float newInterval = (panelWidth - cardWidth * cardCount) / (cardCount - 1);
            spreadInterval = newInterval;
        }
        for (int i = 0; i < cardCount; i++)
        {
            float offset = (i - (cardCount - 1f) / 2) * (cardWidth + spreadInterval);
            panelHandCard.transform.GetChild(i).localPosition = new Vector3(offset, 0, 0);
        }
    }
    void Update()
    {
        //TODO
        RefreshHandCardPos();
    }

    Vector3 curDragCardPosition;
    Vector3 dragDelta;

    public void OnBeginDragCard(UICard uiCard)
    {
        curDragCardPosition = Camera.main.WorldToScreenPoint(uiCard.transform.position);
        dragDelta = curDragCardPosition - Input.mousePosition;
    }

    public void OnDragCard(UICard uiCard)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        MyDebug.Log("mousePosition:" + mousePosition, LogType.Drag);
        uiCard.transform.position = Camera.main.ScreenToWorldPoint(mousePosition + dragDelta);
    }

    public void OnEndDragCard(UICard uiCard)
    {
        uiCard.transform.position = Camera.main.ScreenToWorldPoint(curDragCardPosition);
    }



}