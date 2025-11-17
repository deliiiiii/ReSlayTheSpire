using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[CardID(101)][Serializable]
public class Card101 : CardInTurn
{
    int blockValue => NthEmbedAs<EmbedBlock>(0).BlockValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.GainBlock(blockValue);
        if (TempUpgradeLevel == 0)
        {
            bothTurnData.OpenHandCardOnceClick(1,
                handCard => handCard != this && handCard.CanUpgrade(),
                handCard => handCard.UpgradeTemp());
        }
        else
        {
            bothTurnData.HandList
                .Where(handCard => handCard != this && handCard.CanUpgrade())
                .ForEach(handCard => handCard.UpgradeTemp());
        }
        return UniTask.CompletedTask;
    }
}