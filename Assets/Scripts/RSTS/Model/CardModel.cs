using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSTS;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
public class CardModel : MonoBehaviour
{
    [SerializeReference][ReadOnly] public CardDataBase Data;
    public Text TxtCost;
    public Text TxtName;
    public Text TextCategory;
    public TMP_Text TxtDes;
    public Button Btn;
    Image[] imgs;
    Text[] texts;
    TMP_Text[] tmpTexts;
    public event Action? OnPointerEnterEvt;
    public event Action? OnPointerExitEvt;
    public event Action<Vector3>? OnBeginDragEvt;
    public event Action<Vector3>? OnDragEvt;
    public event Action<Vector3>? OnEndDragEvt;
    public void ReadDataInBothTurn(CardDataBase fData, BothTurnData bothTurnData)
    {
        Data = fData;
        
        TextCategory.text = fData.Config.Category.ToString();
        RefreshTxtCost(bothTurnData);
        RefreshTxtDes(bothTurnData);

        imgs = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<Text>();
        tmpTexts = GetComponentsInChildren<TMP_Text>();
        
        gameObject.SetActive(true);
    }

    public void RefreshTxtDes(BothTurnData bothTurnData)
    {
        string plus = Data.UpgradeLevel switch
        {
            0 => "",
            1 => "+",
            _ => $"+{Data.UpgradeLevel}"
        };
        TxtName.text = $"{Data.Config.Name}{plus}";
        TxtDes.text = bothTurnData.CurContentWithKeywords(Data);
    }

    public void RefreshTxtCost(BothTurnData bothTurnData)
    {
        TxtCost.text = bothTurnData.UIGetEnergy(Data);
    }
    
    public void OnPointerEnter(BaseEventData baseEventData)
    {
        OnPointerEnterEvt?.Invoke();
    }
    public void OnPointerExit(BaseEventData baseEventData)
    {
        OnPointerExitEvt?.Invoke();
    }
    public void OnBeginDragDelegate(BaseEventData baseEventData)
    {
        OnBeginDragEvt?.Invoke(Camera.main!.ScreenToWorldPoint((baseEventData as PointerEventData)!.position));
    }

    public void OnDragDelegate(BaseEventData baseEventData)
    {
        OnDragEvt?.Invoke(Camera.main!.ScreenToWorldPoint((baseEventData as PointerEventData)!.position));
    }
    
    public void OnEndDragDelegate(BaseEventData baseEventData)
    {
        OnEndDragEvt?.Invoke((baseEventData as PointerEventData)!.position);
    }
    
    public void EnableAllShown(bool enable)
    {
        imgs.ForEach(i => i.enabled = enable);
        texts.ForEach(t => t.enabled = enable);
        tmpTexts.ForEach(t => t.enabled = enable);
    }
}

