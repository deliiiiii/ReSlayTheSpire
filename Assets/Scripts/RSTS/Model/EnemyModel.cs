using System;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class EnemyModel : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public EnemyDataBase Data;
    public HPAndBuffModel MdlHPAndBuff;
    public GameObject ImgSelectTarget;
    
    public event Action? OnPointerEnterEvt;
    public event Action? OnPointerExitEvt;

    
    public void ReadData(EnemyDataBase enemyData)
    {
        name = $"Enemy_{enemyData.Config.Name}";
        Binder.FromObs(enemyData.HPAndBuffData.CurHP).To(v =>
        {
            MdlHPAndBuff.SetHP(v, enemyData.Config.MaxHP);
            MdlHPAndBuff.SetShield(0);
        }).Bind();
        MdlHPAndBuff.ReadData(enemyData.HPAndBuffData);
        Data = enemyData;
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