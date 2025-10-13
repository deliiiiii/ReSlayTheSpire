using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

[CreateAssetMenu(menuName = "RSTS/Card", order = 0)][PublicAPI][Serializable]
public class CardConfig : ConfigBase
{
    [OnValueChanged(nameof(OnNameAndIdChanged))]
    [ValidateInput(nameof(CheckName), "名称不能为空")]
    public string Name;
    [OnValueChanged(nameof(OnNameAndIdChanged))]
    [ValidateInput(nameof(CheckId), "ID不能为空且不能为负数")]
    [ValidateInput(nameof(CheckNameAndIdIdentical), "名称_ID：在当前文件夹有重复")]
    public int ID;
    public ECardColor Color;
    public ECardRarity Rarity;
    public ECardCategory Category;
    [ValidateInput(nameof(CheckUpgradeCount), "如果不是状态牌或诅咒牌，升级列表不能为空")]
    public List<CardUpgradeInfo> Upgrades = [];

    bool CheckName() => Name != string.Empty;
    bool CheckId()
    {
        // 判断输入框的ID是否为空
        return ID >= 0;
    }

    bool CheckNameAndIdIdentical()
    {
#if UNITY_EDITOR
        // 尝试寻找与newName同名的文件
        var directoryName = Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this));
        return UnityEditor.AssetDatabase.LoadAssetAtPath<CardConfig>($"{directoryName}/{newName}") == this;
#endif
    }

    bool CheckUpgradeCount()
    {
        if (Category is ECardCategory.State or ECardCategory.Curse)
            return true;
        return Upgrades.Count > 0;
    }

    bool CheckAll()
    {
        return CheckId() 
               && CheckName() 
               && CheckUpgradeCount();
    }

    string newName => $"Card_{ID}_{Name}.asset";
    void OnNameAndIdChanged()
    {
        if (!CheckAll())
            return;
#if UNITY_EDITOR
        // name = newName;
        UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), newName);
#endif
    }

    // [NonSerialized][ShowInInspector] public bool QuickAttack; 
    [NonSerialized][ShowInInspector] public bool QuickState;
    [NonSerialized][ShowInInspector] public bool QuickCurse;

    void OnValidate()
    {
        if (QuickState)
        {
            Category = ECardCategory.State;
            Color = ECardColor.None;
            Rarity = ECardRarity.Special;
            QuickState = false;
            Upgrades.Clear();
        }
        if (QuickCurse)
        {
            Category = ECardCategory.Curse;
            Color = ECardColor.Curse;
            Rarity = ECardRarity.Special;
            QuickCurse = false;
            Upgrades.Clear();
        }
    }
}