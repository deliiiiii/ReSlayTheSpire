using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICard : MonoBehaviour
{
    public Text cardCost;
    public Text cardName;
    public Image cardImage;
    public Text description;
    public Text cardDescription;
    public void ReadData(CardData card)
    {
        cardCost.text = card.IsUpper ? "2" : "1";
        cardName.text = card.CardId.ToString();
        cardImage.color = card.CardColor == CardColor.Red ? Color.red : Color.blue;
        cardDescription.text = "testDescription";
    }
}