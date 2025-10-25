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

    readonly Dictionary<BuffDataBase, BuffModel> dicBuffModel = [];
    
    [Button]
    public void SetHP(int hp, int maxHp)
    {
        txtHP.text = $"{hp.ToString()}/{maxHp.ToString()}";
        imgHP.fillAmount = hp / (float)maxHp;
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
        
        
        data = fData;
    }
}