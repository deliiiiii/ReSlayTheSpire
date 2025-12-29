namespace RSTS;

public partial class YieldCard 
{
    public CardModel? CardModel;
    public EnemyDataBase? Target;
    
    public override void OnExit()
    {
        BelongFSM.HandList.ForEach(card => card.OnExitPlayerYieldCard(this));
    }

    public bool TryYield(Card card, out string failReason)
    {
        failReason = string.Empty;
        if(card.ContainsKeyword(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        if (BelongFSM.CurEnergy < card.Energy)
        {
            failReason = "能量不足";
            return false;
        }
        return card.YieldCondition(BelongFSM, out failReason);
    }
}