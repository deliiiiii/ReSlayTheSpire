using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(5)][Serializable]
public class Card5 : CardInTurn
{
    int block => NthEmbedAs<EmbedBlock>(0).BlockValue;
    int atk => NthEmbedAs<EmbedAttack>(1).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.GainBlock(block);
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}