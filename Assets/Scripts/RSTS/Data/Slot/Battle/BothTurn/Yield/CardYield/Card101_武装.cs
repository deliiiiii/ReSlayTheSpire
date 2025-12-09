using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[Card(101)][Serializable]
public class Card101 : CardInTurn
{
    int BlockValue => BlockAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.GainBlock(BlockValue);
        if (TempUpgradeLevel == 0)
        {
            BothTurn.OpenHandCardOnceClick(1,
                handCard => handCard != Card && handCard.CanUpgrade(),
                handCard => handCard[BothTurn].UpgradeTemp());
        }
        else
        {
            BothTurn.HandList
                .Where(handCard => handCard != Card && handCard.CanUpgrade())
                .ForEach(handCard => handCard[BothTurn].UpgradeTemp());
        }
        return UniTask.CompletedTask;
    }
}