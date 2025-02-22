using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public CardData(int f_cardId, bool f_isUpper)
        {
            CardId = f_cardId;
            IsUpper = f_isUpper;
        }
    }
    public class Card
    {
        public Card(CardData f_cardData)
        {
            CardData = f_cardData;
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


        public Card GetUpperCard()
        {
            return new Card(new CardData(cardData.CardId, true));
        }
        void RefreshInfer()
        {
            CardName = "testCardName";
            CardCost = cardData.IsUpper ? 2 : 1;
            CardColor = CardColor.Red;
            CardType = CardType.Attack;      
        }
    }

    public class UICard : MonoBehaviour
    {
        public Text cardCost;
        public Text cardName;
        public Image cardImage;
        public Text cardDescription;
        public void Refresh(Card card)
        {
            cardCost.text = card.CardCost.ToString();
            cardName.text = card.CardName;
            // cardImage.color = card.CardColor;
            cardDescription.text = "testDescription";
        }
    }

}
