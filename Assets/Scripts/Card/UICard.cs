using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Deli;

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
        // cardImage.sprite = null;
        cardDescription.text = "testDescription";
    }
}