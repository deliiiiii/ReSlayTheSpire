using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class EnemyModel : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public EnemyDataBase Data;
    public HPModel MdlHP;
    public GameObject ImgSelectTarget;
    
    public Action? OnPointerEnterEvt;
    public Action? OnPointerExitEvt;

    
    public void ReadData(EnemyDataBase data)
    {
        name = $"Enemy_{data.Config.Name}";
        Binder.From(data.CurHP).To(v =>
        {
            MdlHP.SetHP(v, data.Config.MaxHP);
            MdlHP.SetShield(0);
        }).Immediate().Bind();

        Data = data;
    }
    
    public void EnableSelectTarget(bool enable)
    {
        ImgSelectTarget.SetActive(enable);
    }
    
    public void OnPointerEnter(PointerEventData _)
    {
        // Debug.Log($"{name} OnPointerEnter");
        OnPointerEnterEvt?.Invoke();
    }

    

    public void OnPointerExit(PointerEventData _)
    {
        // Debug.Log($"{name} OnPointerExit");
        OnPointerExitEvt?.Invoke();
    }
}