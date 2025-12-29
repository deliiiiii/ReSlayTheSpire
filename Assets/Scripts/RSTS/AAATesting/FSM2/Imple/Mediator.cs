using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Cysharp.Threading.Tasks;

namespace RSTS;

public static class Mediator
{
    [Pure]
    public static int Cost(BothTurn bothTurn, Card card) =>
        card.CurCostInfo switch
        {
            CardCostNumber number when card is Card24 => Math.Max(0, number.Cost - bothTurn.LoseHpCount),
            CardCostNumber number => number.Cost,
            CardCostX => bothTurn.CurEnergy,
            CardCostNone or _ => 0,
        };
    
    // public static List<AttackModifyBase> AttackModify(BothTurn bothTurn, Card card)
    //     => card.
    
    public static async UniTask YieldAsync(YieldCard yieldCard, Card card)
    {
        List<YieldContextBase> ycb = card.Config switch
        {
            _ => [],
        };
         foreach (var item in ycb)
             await item.YieldAsync();
    }

    [Pure]
    public static bool TryYield(YieldCard yieldCard, Card card, [NotNullWhen(false)] out string? failReason)
    {
        var bothTurn = yieldCard.BelongFSM;
        failReason = string.Empty;
        if(card.ContainsKeyword(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        if (bothTurn.CurEnergy < Cost(bothTurn, card))
        {
            failReason = "能量不足";
            return false;
        }
        return card.YieldCondition(bothTurn, out failReason);
    }
    
}

public abstract class YieldContextBase
{
    public required YieldCard YieldCard;
    public required Card Card;
    public abstract UniTask YieldAsync();
    
    protected BothTurn BothTurn => YieldCard.BelongFSM;
    protected EnemyDataBase? Target => YieldCard.Target;
}
public class YieldAttack : YieldContextBase
{
    public int Atk;
    public List<AttackModifyBase> ModifyList = [];
    public override UniTask YieldAsync()
    {
        BothTurn.AttackEnemy(Target, Atk);
        return UniTask.CompletedTask;
    }
}
