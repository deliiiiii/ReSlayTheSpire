using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(27)][Serializable]
public class Card27 : Card
{
    int Atk => AtkAt(0);
    int AddHPMax => MiscAt(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        if (bothTurn.AttackEnemyWithResult(target, Atk).AnyType<AttackResultDie>())
        {
            bothTurn.GainMaxHP(AddHPMax);
        }
        return UniTask.CompletedTask;
    }
}