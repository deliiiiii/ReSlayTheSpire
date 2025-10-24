using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class HPModel : MonoBehaviour
{
    [SerializeField] Image imgHP;
    [SerializeField] Text txtHP;
    [SerializeField] Text txtShield;
    [SerializeField] GameObject shield1;
    [SerializeField] GameObject shield2;
    
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
}