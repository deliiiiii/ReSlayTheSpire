using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public Text cardCost;
    public Text cardName;
    public Image cardImage;
    public Text cardDescription;
    public void Refresh(Card card,CardData cardData)
    {
        cardCost.text = card.cardCost.ToString();
        cardName.text = cardData.cardName;
        // cardImage.sprite = null;
        cardDescription.text = "testDescription";
    }
}
