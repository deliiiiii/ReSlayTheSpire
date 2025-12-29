using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[Card(101)][Serializable]
public class Card101 : Card
{
    int BlockValue => BlockAt(0);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.GainBlock(BlockValue);
        if (UpgradeLevel == 0)
        {
            bothTurn.OpenHandCardOnceClick(1,
                handCard => handCard != this && handCard.CanUpgrade(),
                handCard => handCard.UpgradeTemp(bothTurn));
        }
        else
        {
            bothTurn.HandList
                .Where(handCard => handCard != this && handCard.CanUpgrade())
                .ForEach(handCard => handCard.UpgradeTemp(bothTurn));
        }
        return UniTask.CompletedTask;
    }
}