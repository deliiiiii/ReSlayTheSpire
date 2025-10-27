using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
[CreateAssetMenu(menuName = "RSTS/Card", fileName = "Card", order = 1)][PublicAPI][Serializable]
public class CardConfigMulti : ConfigMulti<CardConfigMulti>
{
    public ECardColor Color;
    public ECardRarity Rarity;
    public ECardCategory Category;
    public bool HasTarget;
    [ValidateInput(nameof(CheckUpgradeCount), "升级列表中至少需要一行！")]
    public List<CardUpgradeInfo> Upgrades = [];
    
    
    protected override string PrefixName => "Card";
    protected override bool CheckAll()
    {
        return base.CheckAll() && CheckUpgradeCount();
    }

    [NonSerialized][ShowInInspector] bool quickState;
    [NonSerialized][ShowInInspector] bool quickCurse;
    bool CheckUpgradeCount()
    {
        // if (Category is ECardCategory.State or ECardCategory.Curse)
        //     return true;
        return Upgrades.Count > 0;
    }
    void OnValidate()
    {
        if (quickState)
        {
            Category = ECardCategory.State;
            Color = ECardColor.None;
            Rarity = ECardRarity.Special;
            quickState = false;
            Upgrades.Clear();
        }
        if (quickCurse)
        {
            Category = ECardCategory.Curse;
            Color = ECardColor.Curse;
            Rarity = ECardRarity.Curse;
            quickCurse = false;
            Upgrades.Clear();
        }
    }
}

#region Cost
public abstract class CardCostBase;
[Serializable][PublicAPI]
public class CardCostNumber : CardCostBase
{
    public int Cost;
}
[Serializable]
public class CardCostX : CardCostBase;
[Serializable]
public class CardCostNone : CardCostBase;
#endregion

#region Upgrade
[Serializable]
public class CardUpgradeInfo
{
    public EmbedString Des;
    [ValueDropdown(nameof(GetKeywords), IsUniqueList = true)]
    public List<ECardKeyword> Keywords = [];
    [SerializeReference]
    public CardCostBase CostInfo = new CardCostNumber();

    public string ContentWithKeywords
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            bool findPre = false;
            if (Keywords.Any(k => k == ECardKeyword.Unplayable))
            {
                findPre = true;
                sb.Append("[不可打出]");
            }
            if (Keywords.Any(k => k == ECardKeyword.Inborn))
            {
                findPre = true;
                sb.Append("[固有]");
            }
            if (findPre)
                sb.AppendLine();
            sb.Append(Des.Content);
            if (Keywords.Any(k => k == ECardKeyword.Retain))
            {
                findPre = true;
                sb.Append("[保留]");
            }
            if (Keywords.Any(k => k == ECardKeyword.Ethereal))
            {
                findPre = true;
                sb.Append("[虚无]");
            }
            if (Keywords.Any(k => k == ECardKeyword.Exhaust))
            {
                findPre = true;
                sb.Append("[消耗]");
            }
            return sb.ToString();
        }
    }

    List<ECardKeyword> GetKeywords() => [..(ECardKeyword[])Enum.GetValues(typeof(ECardKeyword))];
}
#endregion

[Serializable]
public class EmbedString
{
    public static Dictionary<Type, string> EmbedCharPairs = new()
    {
        { typeof(int), "[]" },
        { typeof(EnergyInt), "{}"},
        // { EmbedType.Float, new EmbedCharPair() { Left = "[[", Right = "]]" } },
    }; 
    [SerializeField]
    string content;

    public string Content
    {
        get
        {
            StringBuilder sb = new StringBuilder(content);
            // 替换Data中[],{}为Config中对应的值
            EmbedCharPairs.ForEach(pair =>
            {
                if (pair.Key == typeof(int))
                {
                    if (!CheckIntCount())
                        return;
                    int trueCount = EmbedIntList.Count;
                    int c = 0;
                    while (c < trueCount)
                    {
                        ReplaceFirst(sb.ToString(), pair.Value, EmbedIntList[c].ToString(), out var result);
                        sb.Clear();
                        sb.Append(result);
                        c++;
                    }
                }

                if (pair.Key == typeof(EnergyInt))
                {
                    int trueCount = EmbedEnergyIntList.Count;
                    int c = 0;
                    while (c < trueCount)
                    {
                        ReplaceFirst(sb.ToString(), pair.Value, $"*{EmbedEnergyIntList[c].ToString()}费*", out var result);
                        sb.Clear();
                        sb.Append(result);
                        c++;
                    }
                }
            });
            return sb.ToString();
        }
    }
    
    // 在编辑器中显示另一个文本的Attribute
    [Header("嵌入Int使用[]")][HideLabel]// [ValidateInput(nameof(CheckIntCount), "嵌入的Int数量与Content中的[]数量不匹配")]
    public List<int> EmbedIntList = [];
    [Header("嵌入能量数量使用{}")][HideLabel]// [ValidateInput(nameof(CheckIntCount), "嵌入的Int数量与Content中的[]数量不匹配")]
    public List<int> EmbedEnergyIntList = [];
    
    bool CheckIntCount() => SubStringCount(content, EmbedCharPairs[typeof(int)]) == EmbedIntList.Count;
    bool CheckEnergyIntCount() => SubStringCount(content, EmbedCharPairs[typeof(EnergyInt)]) == EmbedEnergyIntList.Count;
    int SubStringCount(string main, string sub)
    {
        var count = 0;
        var pos = 0;
        while ((pos = main.IndexOf(sub, pos, StringComparison.Ordinal)) != -1)
        {
            count++;
            pos += sub.Length;
        }
        return count;
    }
    
    bool ReplaceFirst(string main, string replaceFrom, string replaceTo, out string result)
    {
        result = main;
        int pos = main.IndexOf(replaceFrom, StringComparison.Ordinal);
        if (pos < 0)
            return false;
        result = main[..pos] + replaceTo + main[(pos + replaceFrom.Length)..];
        return true;
    }
}

public class EnergyInt;