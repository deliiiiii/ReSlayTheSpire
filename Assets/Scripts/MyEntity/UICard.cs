using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Text cardCost;
    public Text cardName;
    public Image cardImage;
    public Text description;
    public Text cardDescription;

    Vector3 originalScale;
    int originalIndex;

    public bool IsHandCard;
    public void ReadData(CardData card)
    {
        cardCost.text = card.IsUpper ? "2" : "1";
        cardName.text = card.CardId.ToString();
        cardImage.color = card.CardColor == CardColor.Red ? Color.red : Color.blue;
        cardDescription.text = "testDescription";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!IsHandCard)
        {
            return;
        }
        originalScale = transform.localScale;
        originalIndex = transform.GetSiblingIndex();
        transform.localScale = originalScale * 1.2f;
        transform.SetSiblingIndex(transform.parent.childCount);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(!IsHandCard)
        {
            return;
        }
        transform.localScale = originalScale;
        transform.SetSiblingIndex(originalIndex);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(!IsHandCard)
        {
            return;
        }
        GlobalView.BattleView.OnDragCard(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsHandCard)
        {
            return;
        }
        GlobalView.BattleView.OnBeginDragCard(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!IsHandCard)
        {
            return;
        }
        GlobalView.BattleView.OnEndDragCard(this);
    }
}


