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
    [SerializeReference][ReadOnly]
    CardDataBase data;
    public Text TxtCost;
    public Text TxtName;
    public Text TextCategory;
    public TMP_Text TxtDes;
    public Action? OnPointerEnterEvt;
    public Action? OnPointerExitEvt;
    public Action<Vector3>? OnBeginDragEvt;
    public Action<Vector3>? OnDragEvt;
    public Action<Vector3>? OnEndDragEvt;
    public void InitByData(CardDataBase fData)
    {
        data = fData;
        TxtCost.text = fData.CurCostInfo switch
        {
            CardCostNumber costNumber => costNumber.Cost.ToString(),
            CardCostX => "X",
            CardCostNone or _=> "",
        };
        TxtName.text = fData.Config.Name;
        TextCategory.text = fData.Config.Category.ToString();
        TxtDes.text = fData.CurDes.Content;
        
        var evtTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        EventTrigger.Entry entryBeginDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag
        };
        EventTrigger.Entry entryDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag
        };

        entryPointerEnter.callback.AddListener(baseEventData => OnPointerEnter((PointerEventData)baseEventData));
        entryPointerExit.callback.AddListener(baseEventData => OnPointerExit((PointerEventData)baseEventData));
        entryBeginDrag.callback.AddListener(baseEventData => OnBeginDragDelegate((PointerEventData)baseEventData));
        entryDrag.callback.AddListener(baseEventData => OnDragDelegate((PointerEventData)baseEventData));
        entryEndDrag.callback.AddListener(baseEventData => OnEndDragDelegate((PointerEventData)baseEventData));
        
        evtTrigger.triggers.Add(entryPointerEnter);
        evtTrigger.triggers.Add(entryPointerExit);
        evtTrigger.triggers.Add(entryBeginDrag);
        evtTrigger.triggers.Add(entryDrag);
        evtTrigger.triggers.Add(entryEndDrag);

        imgs = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<Text>();
        tmpTexts = GetComponentsInChildren<TMP_Text>();
    }

    
    Image[] imgs;
    Text[] texts;
    TMP_Text[] tmpTexts;
    
    void OnPointerEnter(PointerEventData baseEventData)
    {
        OnPointerEnterEvt?.Invoke();
    }
    void OnPointerExit(PointerEventData baseEventData)
    {
        OnPointerExitEvt?.Invoke();
    }
    void OnBeginDragDelegate(PointerEventData baseEventData)
    {
        OnBeginDragEvt?.Invoke(Camera.main!.ScreenToWorldPoint(baseEventData.position));
        EnableAllShown(false);
    }

    void OnDragDelegate(PointerEventData baseEventData)
    {
        OnDragEvt?.Invoke(Camera.main!.ScreenToWorldPoint(baseEventData.position));
    }
    
    void OnEndDragDelegate(PointerEventData baseEventData)
    {
        OnEndDragEvt?.Invoke(baseEventData.position);
        EnableAllShown(true);
    }
    
    void EnableAllShown(bool enable)
    {
        imgs.ForEach(i => i.enabled = enable);
        texts.ForEach(t => t.enabled = enable);
        tmpTexts.ForEach(t => t.enabled = enable);
    }
}

