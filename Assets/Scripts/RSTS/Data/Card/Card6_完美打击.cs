using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(6)][Serializable]
public class Card6 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int addPerDaJi => NthEmbedAs<EmbedMisc>(1).MiscValue;
    public int BaseAtkAddByDaJi(BothTurnData bothTurnData) => addPerDaJi * bothTurnData.DaJiCount;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk + BaseAtkAddByDaJi(bothTurnData));
        return UniTask.CompletedTask;
    }
}