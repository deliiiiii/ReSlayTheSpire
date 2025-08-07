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
    [HideInInspector]
    public GameObject? DetailPnlShown;

    public event Action? OnPointerEnterEvt;
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvt?.Invoke();
        DetailPnlShown?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DetailPnlShown?.SetActive(false);
    }
}