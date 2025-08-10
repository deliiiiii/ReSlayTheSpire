using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Violee.View;

public class RoomIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public required Text TitleTxt;
    public required Button Btn;
    public required Image RoomImg;
    public required GameObject DesPnl;
    public required Text DesTxt;
    public void OnPointerEnter(PointerEventData eventData)
    {
        DesPnl.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DesPnl.gameObject.SetActive(false);
    }
}