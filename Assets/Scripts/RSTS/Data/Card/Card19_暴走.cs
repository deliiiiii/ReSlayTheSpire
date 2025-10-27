using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(19)][Serializable]
public class Card19 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int addPerUse => NthEmbedAs<EmbedMisc>(1).MiscValue;
    int useTime;
    public int BaseAtkAdd => addPerUse * useTime;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk + BaseAtkAdd);
        useTime++;
        return UniTask.CompletedTask;
    }

    public override void OnExitBothTurn()
    {
        useTime = 0;
    }
}
