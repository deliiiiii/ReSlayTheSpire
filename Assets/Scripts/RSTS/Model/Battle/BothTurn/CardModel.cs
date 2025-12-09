using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSTS;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
public class CardModel : MonoBehaviour
{
    [SerializeReference][ReadOnly] public Card Data;
    BothTurn bothTurn;
    public Text TxtCost;
    public Text TxtName;
    public Text TextCategory;
    public TMP_Text TxtDes;
    public Button Btn;
    Image[] imgs;
    Text[] texts;
    TMP_Text[] tmpTexts;
    public UnityEvent OnPointerEnterEvt = new();
    public UnityEvent OnPointerExitEvt = new();
    public UnityEvent<Vector3> OnBeginDragEvt = new();
    public UnityEvent<Vector3> OnDragEvt = new();
    public UnityEvent<Vector3> OnEndDragEvt = new();
    public void ReadDataInBothTurn(Card fData, BothTurn fBothTurn)
    {
        bothTurn = fBothTurn;
        Data = fData;
        
        TextCategory.text = Data.Config.Category.ToString();
        RefreshTxtCost();
        RefreshTxtDes();

        imgs = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<Text>();
        tmpTexts = GetComponentsInChildren<TMP_Text>();
        
        gameObject.SetActive(true);
    }
    
    public void RefreshTxtDes()
    {
        // TODO 根据当前状态调用OfTurn...
        string plus = Data.UpgradeLevel switch
        {
            0 => "",
            1 => "+",
            _ => $"+{Data.UpgradeLevel}"
        };
        TxtName.text = $"{Data.Config.Name}{plus}";
        // TODO
        TxtDes.text = Data.ContentWithKeywords;
    }

    public void RefreshTxtCost()
    {
        TxtCost.text = Data.UIEnergy;
    }
    
    public void OnPointerEnter(BaseEventData baseEventData)
    {
        OnPointerEnterEvt.Invoke();
    }
    public void OnPointerExit(BaseEventData baseEventData)
    {
        OnPointerExitEvt.Invoke();
    }
    public void OnBeginDragDelegate(BaseEventData baseEventData)
    {
        OnBeginDragEvt.Invoke(Camera.main!.ScreenToWorldPoint((baseEventData as PointerEventData)!.position));
    }

    public void OnDragDelegate(BaseEventData baseEventData)
    {
        OnDragEvt.Invoke(Camera.main!.ScreenToWorldPoint((baseEventData as PointerEventData)!.position));
    }
    
    public void OnEndDragDelegate(BaseEventData baseEventData)
    {
        OnEndDragEvt.Invoke((baseEventData as PointerEventData)!.position);
    }
    
    public void EnableAllShown(bool enable)
    {
        imgs.ForEach(i => i.enabled = enable);
        texts.ForEach(t => t.enabled = enable);
        tmpTexts.ForEach(t => t.enabled = enable);
    }
}

