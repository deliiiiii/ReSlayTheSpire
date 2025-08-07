using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Violee.View;

public class ConsistentBuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public required Image Image;
    public required Text DetailTxt;
    public required GameObject DetailPnl;

    public void OnPointerEnter(PointerEventData eventData)
    {
        DetailPnl.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DetailPnl.SetActive(false);
    }
}