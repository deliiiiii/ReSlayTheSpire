using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deli
{
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
        public int CardId;
        public bool IsUpper;
    }
    public class Card
    {
        public Card(CardData f_cardData)
        {
            cardData = f_cardData;
        }

        CardData cardData;
        public CardData CardData
        {
            get
            {
                return cardData;
            }
            set
            {
                cardData = value;
                RefreshInfer();
            }
        }

        #region Infer 根据CardData读取
        public string CardName;
        public int CardCost;
        public CardColor CardColor;
        public CardType CardType;
        #endregion

        #region 根据CardData和上面一个region内的变量读取
        UICard uiCard;

        void RefreshInfer()
        {
            CardName = "testCardName";
            CardCost = cardData.IsUpper ? 2 : 1;
            CardColor = CardColor.Red;
            CardType = CardType.Attack;
            uiCard.Refresh(this);
        }
    }
}
