using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS;
using RSTS.CDMV;

public class Card
{
    protected Card(){}
    public Battle Battle { get; private set; } = null!;
    public static Card Create(Battle battle, int id)
    {
        var ret = new Card
        {
            Battle = battle,
            Config = RefPoolMulti<CardConfigMulti>.Acquire()
                .FirstOrDefault(c => c.ID == id)!,
            cardID = id
        };
        return ret;
    }
    [JsonIgnore] public CardConfigMulti Config = null!;
    [JsonProperty] int cardID;

    public int UpgradeLevel;

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
    public T NthEmbedAs<T>(int id) where T : EmbedType => (CurUpgradeInfo.Des.EmbedTypes[id] as T)!;
    public TBuff NthEmbedAsBuffCopy<TBuff>(int id) where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();
    
    // TODO UI
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

    CardInTurn? inTurn;
    public CardInTurn this[BothTurn bothTurn] => inTurn!;
    public CardInTurn EnterTurn(BothTurn bothTurn) => inTurn = CardInTurn.CreateFromBattle(this, bothTurn);
    public void ExitTurn(BothTurn bothTurn) => inTurn = null;

}

public abstract class CardInTurn : DataAttr<CardInTurn, CardAttribute>
{
    public static CardInTurn CreateFromBattle(Card card, BothTurn bothTurn)
    {
        var ret = CreateByAttr(card.Config.ID);
        ret.Card = card;
        ret.BothTurn = bothTurn;
        return ret;
    }

    protected static Card CreateNowhereCard(int id, BothTurn bothTurn)
    {
        var ret = Card.Create(bothTurn.BelongFSM, id);
        ret.EnterTurn(bothTurn);
        return ret;
    }

    protected Card Card { get; private set; } = null!;
    protected BothTurn BothTurn { get; private set; } = null!;
    public int TempUpgradeLevel;
    public bool CanUpgrade() => TempUpgradeLevel < Card.Config.Upgrades.Count - 1;
    // 临时加入的，如“愤怒”
    public bool IsTemporary;
    public void UpgradeTemp()
    {
        TempUpgradeLevel++;
        OnTempUpgrade?.Invoke();
    }
    public event Action? OnTempUpgrade;
    
    
    public int Energy
    {
        get
        {
            return Card.CurCostInfo switch
            {
                CardCostNumber number when this is Card24 => Math.Max(0, number.Cost - BothTurn.LoseHpCount),
                CardCostNumber number => number.Cost,
                CardCostX => BothTurn.CurEnergy,
                CardCostNone or _ => 0,
            };
        }
    }
    
    // TODO UI
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
        
        
    public abstract UniTask YieldAsync(int cost, EnemyDataBase? target);
    public virtual bool YieldCondition(out string failReason)
    {
        failReason = string.Empty;
        return true;
    }
    public virtual void OnExitPlayerYieldCard() {}
    protected virtual List<AttackModifyBase> ModifyList => [];
        
    protected int AtkAt(int id) => Card.NthEmbedAs<EmbedAttack>(id).AttackValue;
    protected int AtkTimeAt(int id) => Card.NthEmbedAs<EmbedAttackTime>(id).AttackTimeValue;
    protected int BlockAt(int id) => Card.NthEmbedAs<EmbedBlock>(id).BlockValue;
    protected int DrawAt(int id) => Card.NthEmbedAs<EmbedDraw>(id).DrawValue;
    protected int EnergyAt(int id) => Card.NthEmbedAs<EmbedEnergy>(id).EnergyValue;
    protected TBuff BuffAt<TBuff>(int id) where TBuff : BuffDataBase => Card.NthEmbedAsBuffCopy<TBuff>(id);
    protected int MiscAt(int id) => Card.NthEmbedAs<EmbedMisc>(id).MiscValue;
}