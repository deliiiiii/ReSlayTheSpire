using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS;
using RSTS.CDMV;

// Card.XXXProperty : (Context,Card) -> type
//                 | (Battle,Card) -> type
//                 | (BothTurn,Card) -> type
public abstract partial class Card : DataAttr<Card, CardAttribute>
{
    public static Card Create(int id)
    {
        var ret = CreateByAttr(id);
        ret.Config = RefPoolMulti<CardConfigMulti>.Acquire().FirstOrDefault(c => c.ID == id)!;
        return ret;
    }
    [JsonIgnore] public CardConfigMulti Config = null!;

    public int UpgradeLevel
    {
        get => isTempUpgraded ? tempUpgradeLevel : field;
        private set
        {
            if(isTempUpgraded)
                tempUpgradeLevel = value;
            else
                field = value;
        }
    }
    
    bool isTempUpgraded;
    int tempUpgradeLevel;
    // 临时加入的，如“愤怒”
    public bool IsTemporary;
    public void UpgradeTemp(BothTurn bothTurn)
    {
        if (!CanUpgrade())
            return;
        isTempUpgraded = true;
        tempUpgradeLevel++;
        // OnUpgrade?.Invoke();
    }
    public void Upgrade()
    {
        if (!CanUpgrade())
            return; 
        UpgradeLevel++;
        // OnUpgrade?.Invoke();
    }
    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    public bool CanUpgrade() => UpgradeLevel < Config.Upgrades.Count - 1;
    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;
    public EmbedString CurDes => CurUpgradeInfo.Des;
    public bool HasTarget => Config.HasTarget;
    T NthEmbedAs<T>(int id) where T : EmbedType => (CurUpgradeInfo.Des.EmbedTypes[id] as T)!;
    TBuff NthEmbedAsBuffCopy<TBuff>(int id) where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();
    
    public abstract UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target);
    public virtual bool YieldCondition(BothTurn bothTurn, out string failReason)
    {
        failReason = string.Empty;
        return true;
    }
    public virtual void OnExitPlayerYieldCard(YieldCard yieldCard) {}
    protected virtual List<AttackModifyBase> GetModifyList(BothTurn bothTurn) => [];

    protected int AtkAt(int id) => NthEmbedAs<EmbedAttack>(id).AttackValue;
    protected int AtkTimeAt(int id) => NthEmbedAs<EmbedAttackTime>(id).AttackTimeValue;
    protected int BlockAt(int id) => NthEmbedAs<EmbedBlock>(id).BlockValue;
    protected int DrawAt(int id) => NthEmbedAs<EmbedDraw>(id).DrawValue;
    protected int EnergyAt(int id) => NthEmbedAs<EmbedEnergy>(id).EnergyValue;
    protected TBuff BuffAt<TBuff>(int id) where TBuff : BuffDataBase => NthEmbedAsBuffCopy<TBuff>(id);
    protected int MiscAt(int id) => NthEmbedAs<EmbedMisc>(id).MiscValue;
    
    
    
    // TODO UI 函数输入应为 Battle 和 Card
    // public string UIEnergy
    // {
    //     get
    //     {
    //         return CurCostInfo switch
    //         {
    //             CardCostNumber costNumber => costNumber.Cost.ToString(),
    //             CardCostX => "X",
    //             CardCostNone or _ => "",
    //         };
    //     }
    // }
    // public string ContentWithKeywords => CurUpgradeInfo.ContentWithKeywords([]);
    
    // TODO UI 函数输入应为 BothTurn 和 Card
    // public string UIEnergy
    // {
    //     get
    //     {
    //         if (this is Card24 && Card.CurCostInfo is CardCostNumber number)
    //         {
    //             return Math.Max(0, number.Cost - BothTurn.LoseHpCount).ToString();
    //         }
    //         return Card.UIEnergy;
    //     }
    // }
    
    // TODO UI 函数输入应为 ?? 和 Card
    // public string ContentWithKeywords
    // {
    //     get
    //     {
    //         // var isYieldCard = BothTurn.IsState<YieldCard>(out var yieldCard);
    //         // var target = isYieldCard ? yieldCard.Target : null;
    //         
    //         var replacerList = new List<string>();
    //         Card.CurDes.EmbedTypes.ForEach(embedType =>
    //         {
    //             replacerList.Add(embedType switch
    //             {
    //                 IEmbedNotChange notChange => notChange.GetNotChangeString(),
    //                 EmbedCard6 => GetModify<AttackModifyCard6>().BaseAtkAddByDaJi.ToString(),
    //                 // EmbedCard12 => cardData.GetModify<AttackModifyCard12>(this).AtkByBlock.ToString(),
    //                 EmbedCard19 => GetModify<AttackModifyCard19>().BaseAtkAddByUse.ToString(),
    //                 EmbedCard28 => GetModify<AttackModifyCard28>().AtkTimeByExhaust.ToString(),
    //                 // TODO 打牌时攻击值计算
    //                 // EmbedAttack attack =>
    //                 //     Outer.GetAttackValue(BothTurn.PlayerHPAndBuffData, target?.HPAndBuffData, attack.AttackValue
    //                 //         , ModifyList).ToString(),
    //                 _ => "NaN!"
    //             });
    //         });
    //         return Card.CurUpgradeInfo.ContentWithKeywords(replacerList);
    //     }
    // }
    // protected T GetModify<T>() where T : AttackModifyBase => ModifyList.OfType<T>().FirstOrDefault()!;

}

public abstract partial class Card
{
    public void OnEnterBothTurn(BothTurn bothTurn) {}
    public void OnExitBothTurn(BothTurn bothTurn) {}
}