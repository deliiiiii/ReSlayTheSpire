using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    
    public string ContentWithKeywords(List<string> replacerList)
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
        sb.Append(Des.ReplacedContent(replacerList));
        if (Keywords.Any(k => k == ECardKeyword.Retain))
        {
            sb.Append("[保留]");
        }
        if (Keywords.Any(k => k == ECardKeyword.Ethereal))
        {
            sb.Append("[虚无]");
        }
        if (Keywords.Any(k => k == ECardKeyword.Exhaust))
        {
            sb.Append("[消耗]");
        }
        return sb.ToString();
    }

    List<ECardKeyword> GetKeywords() => [..(ECardKeyword[])Enum.GetValues(typeof(ECardKeyword))];
}
#endregion

[Serializable]
public class EmbedString
{
    [SerializeField] string content;
    public string ReplacedContent(List<string> replacerList)
    {
        StringBuilder sb = new StringBuilder(content);
        // 替换Data中[]为Config中对应的值
        int trueCount = EmbedTypes.Count;
        int c = 0;
        while (c < trueCount)
        {
            ReplaceFirst(sb.ToString(), "[]", replacerList[c], out var result);
            sb.Clear();
            sb.Append(result);
            c++;
        }
        return sb.ToString();
    }
    
    // 在编辑器中显示另一个文本的Attribute
    [Header("嵌入数值都使用[]，这个list填入多态子类！")][HideLabel][SerializeReference]
    // [ValidateInput(nameof(CheckEmbedCount), "嵌入的Int数量与Content中的[]数量不匹配")]
    public List<EmbedType> EmbedTypes = [];
    
    bool CheckEmbedCount() => SubStringCount(content, "[]") == EmbedTypes.Count;
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
[Serializable]
public abstract class EmbedType;

public interface IEmbedNotChange
{
    public string GetNotChangeString();
}

[Serializable]
public class EmbedAttack : EmbedType
{
    public int AttackValue;
}
[Serializable]
public class EmbedAttackTime : EmbedType, IEmbedNotChange
{
    public int AttackTimeValue;
    public string GetNotChangeString()
    {
        return AttackTimeValue.ToString();
    }
}
[Serializable]
public class EmbedBlock : EmbedType, IEmbedNotChange
{
    public int BlockValue;
    public string GetNotChangeString()
    {
        return BlockValue.ToString();
    }
}
[Serializable]
public class EmbedDraw : EmbedType, IEmbedNotChange
{
    public int DrawValue;
    public string GetNotChangeString()
    {
        return DrawValue.ToString();
    }
}
[Serializable]
public class EmbedEnergy : EmbedType, IEmbedNotChange
{
    public int EnergyValue;
    public string GetNotChangeString()
    {
        return EnergyValue.ToString();
    }
}
[Serializable]
public class EmbedAddBuff : EmbedType, IEmbedNotChange
{
    [SerializeReference]
    public BuffDataBase BuffData;

    public string GetNotChangeString()
    {
        return BuffData.StackCount.ToString();
    }
}

[Serializable]
public class EmbedMisc : EmbedType, IEmbedNotChange
{
    public int MiscValue;
    public string GetNotChangeString()
    {
        return MiscValue.ToString();
    }
}

[Serializable]
public class EmbedCard6 : EmbedType;
[Serializable]
public class EmbedCard19 : EmbedType;
[Serializable]
public class EmbedCard28 : EmbedType;