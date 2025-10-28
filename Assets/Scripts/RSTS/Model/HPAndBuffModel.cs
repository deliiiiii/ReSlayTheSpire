using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class HPAndBuffModel : MonoBehaviour
{
    [SerializeReference] HPAndBuffData data;
    [SerializeField] Image imgHP;
    [SerializeField] Text txtHP;
    [SerializeField] Text txtShield;
    [SerializeField] GameObject shield1;
    [SerializeField] GameObject shield2;
    [SerializeField] Transform transBuff;
    [SerializeField] BuffModel pfbBuffModel;
    [SerializeField] HurtEffect pfbHurtEffect;

    readonly Dictionary<BuffDataBase, BuffModel> dicBuffModel = [];
    
    [Button]
    public void DirectlySetHP(int hp, int maxHp)
    {
        txtHP.text = $"{hp.ToString()}/{maxHp.ToString()}";
        imgHP.fillAmount = hp / (float)maxHp;
    }

    void OnHurt(int hurt)
    {
        var hurtEffect = Instantiate(pfbHurtEffect, transform);
        hurtEffect.TxtHurt.text = $"{hurt}";
        hurtEffect.TxtHurt.color = Color.red;
        hurtEffect.gameObject.SetActive(true);
    }

    void OnHeal(int heal)
    {
        var hurtEffect = Instantiate(pfbHurtEffect, transform);
        hurtEffect.TxtHurt.text = $"{heal}";
        hurtEffect.TxtHurt.color = Color.green;
        hurtEffect.gameObject.SetActive(true);
    }
    
    

    [Button]
    public void SetShield(int shield)
    {
        if (shield > 0)
        {
            shield1.SetActive(true);
            shield2.SetActive(true);
            txtShield.text = shield.ToString();
            return;
        }
        shield1.SetActive(false);
        shield2.SetActive(false);
    }

    public void ReadData(HPAndBuffData fData)
    {
        DirectlySetHP(fData.CurHP, fData.MaxHP);
        SetShield(0);
        fData.OnAddBuff += buffData =>
        {
            var buffModel = Instantiate(pfbBuffModel, transBuff);
            buffModel.ReadData(buffData);
            buffModel.gameObject.SetActive(true);
            dicBuffModel.Add(buffData, buffModel);
        };
        fData.OnRemoveBuff += buffData =>
        {
            var buffModel = dicBuffModel[buffData];
            Destroy(buffModel.gameObject);
            dicBuffModel.Remove(buffData);
        };

        fData.OnChangeBuffStack += (buffData, count) =>
        {
            var buffModel = dicBuffModel[buffData];
            buffModel.ChangeCount(count);
        };
        
        fData.CurHP.OnValueChangedFull += (oldV, newV) =>
        {
            DirectlySetHP(newV, fData.MaxHP);
            switch (newV - oldV)
            {
                case > 0:
                    OnHeal(newV - oldV);
                    break;
                case < 0:
                    OnHurt(oldV - newV);
                    break;
            }
        };
        
        data = fData;
    }
}