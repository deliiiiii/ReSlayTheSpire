using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class EnemyModel : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public EnemyDataBase Data;
    public HPAndBuffModel MdlHPAndBuff;
    public GameObject ImgSelectTarget;
    
    public UnityEvent OnPointerEnterEvt = new();
    public UnityEvent OnPointerExitEvt = new();

    
    public void ReadData(EnemyDataBase enemyData)
    {
        Data = enemyData;
        
        name = $"Enemy_{Data.Config.Name}";
        MdlHPAndBuff.ReadData(Data.HPAndBuffData);
        
    }
    
    public void EnableSelectTarget(bool enable)
    {
        ImgSelectTarget.SetActive(enable);
    }
    
    public void OnPointerEnter(PointerEventData _)
    {
        // Debug.Log($"{name} OnPointerEnter");
        OnPointerEnterEvt.Invoke();
    }

    

    public void OnPointerExit(PointerEventData _)
    {
        // Debug.Log($"{name} OnPointerExit");
        OnPointerExitEvt.Invoke();
    }
}