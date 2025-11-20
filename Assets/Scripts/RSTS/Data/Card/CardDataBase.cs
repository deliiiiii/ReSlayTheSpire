using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS.CDMV;
namespace RSTS;

[Serializable]
public class CardData
{
    public CardData(int id){ID = id;}
    [JsonIgnore] public CardConfigMulti Config = null!;
    [JsonProperty]
    int ID
    {
        set
        {
            field = value;
            Config = RefPoolMulti<CardConfigMulti>.Acquire()
                .FirstOrDefault(c => c.ID == field)!;
        }
    }
    
    public int UpgradeLevel
    {
        get => InTurn?.TempUpgradeLevel ?? field;
        set
        {
            if (InTurn != null)
                InTurn.TempUpgradeLevel = value;
            else
                field = value;
        }
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
    public T NthEmbedAs<T>(int id)
        where T : EmbedType
        => (CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as T)!;

    public TBuff NthEmbedAsBuffCopy<TBuff>(int id)
        where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();
    
    // [SubState<EBattleState>(EBattleState.BothTurn)]
    public CardInTurn? InTurn
    {
        get
        {
            if (FSM.GameData.IsSubState<BattleData>(out var battleData) &&
                battleData.IsSubState<BothTurnData>(out _))
                return field;
            // MyDebug.LogError("CardInTurn 只能在战斗中访问");
            return null;
        }
        set;
    }
}

[Serializable]
public abstract class CardInTurn : 
    DataAttr<CardInTurn, CardIDAttribute, CardData>, IBelong<CardData>
{
    // 反射初始化
    public CardData Parent { get; private set; } = null!;
    public T NthEmbedAs<T>(int id) where T : EmbedType => Parent.NthEmbedAs<T>(id);
    public TBuff NthEmbedAsBuffCopy<TBuff>(int id) where TBuff : BuffDataBase => Parent.NthEmbedAsBuffCopy<TBuff>(id);
    
    
    // 无来源卡牌
    public CardInTurn CreateBlindCard(int id) => CreateByAttr(id, Parent);
    
    public int TempUpgradeLevel;
    public EnemyDataBase? Target;
    // 临时加入的，如“愤怒”
    public bool IsTemporary;

    protected override void ReadContext(CardData context)
    {
        Parent = context;
        Parent.InTurn = this;
        TempUpgradeLevel = context.UpgradeLevel;
    }

    public event Action? OnTempUpgrade;
    public virtual List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData) => [];
    public virtual bool YieldCondition(BothTurnData bothTurnData, out string failReason)
    {
        failReason = string.Empty;
        return true;
    }
    public virtual void OnEnterBothTurn() { }
    public virtual void OnExitBothTurn() { }
    public virtual void OnPlayerTurnEnd(BothTurnData bothTurnData){}
    // public virtual bool RecommendYield(BothTurnData bothTurnData) => false;
    public bool CanUpgrade() => Parent.CanUpgrade();

    public void UpgradeTemp()
    {
        TempUpgradeLevel++;
        OnTempUpgrade?.Invoke();
    }
    public T GetModify<T>(BothTurnData bothTurnData)
        where T : AttackModifyBase
    {
        return GetModifyList(bothTurnData).OfType<T>().FirstOrDefault()!;
    }
    public abstract UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy);
}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : IDAttribute(id);