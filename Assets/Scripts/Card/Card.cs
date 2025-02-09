using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardColor
{
    None,
    Red,
}
public enum CardType
{
    Attack,
    Skill,
    Ability,
    Curse,
}
[Serializable]
public class CardData
{
    public string cardName;
    public bool isUpper;
}
public class Card: IDestroy
{
    public void MyDestroy()
    {
        GameObject.Destroy(UICard);
    }

    public Card(CardData f_cardData)
    {
        CardData = f_cardData;
    }

    CardData cardData;
    CardData CardData
    {
        get
        {
            MyDebug.Log("CardData get", LogType.Card);
            return cardData;
        }
        set
        {
            MyDebug.Log("CardData set", LogType.Card);
            cardData = value;
            RefreshData();
        }
    }

    #region 根据CardData读取
    public int cardCost;
    public CardColor cardColor;
    public CardType cardType;
    #endregion

    #region 根据CardData和上面一个region内的变量读取
    UICard uiCard;
    UICard UICard
    {
        get
        {
            if (uiCard == null)
            {
                uiCard = GameObject.Instantiate(MyResources.Load<UICard>(ResourcePath.UICard)); 
            }
            return uiCard;
        }
    }
    #endregion

    void RefreshData()
    {
        cardCost = CardData.isUpper ? 2 : 1;
        cardColor = CardColor.Red;
        cardType = CardType.Attack;
        UICard.Refresh(this, CardData);
    }
}
