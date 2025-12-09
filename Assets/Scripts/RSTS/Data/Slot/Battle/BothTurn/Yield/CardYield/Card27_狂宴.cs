using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(27)][Serializable]
public class Card27 : CardInTurn
{
    int Atk => AtkAt(0);
    int AddHPMax => MiscAt(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        if (BothTurn.AttackEnemyWithResult(target, Atk).AnyType<AttackResultDie>())
        {
            BothTurn.GainMaxHP(AddHPMax);
        }
        return UniTask.CompletedTask;
    }
}