using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;

public partial class YieldCard 
{
    public CardModel? CardModel;
    public EnemyDataBase? Target;
    
    public void OnExit()
    {
        BelongFSM.HandList.ForEach(card => card[BelongFSM].OnExitPlayerYieldCard());
    }

    public bool TryYield(Card card, out string failReason)
    {
        failReason = string.Empty;
        if(card.ContainsKeyword(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        if (BelongFSM.CurEnergy < card[BelongFSM].Energy)
        {
            failReason = "能量不足";
            return false;
        }
        return card[BelongFSM].YieldCondition(out failReason);
    }
}