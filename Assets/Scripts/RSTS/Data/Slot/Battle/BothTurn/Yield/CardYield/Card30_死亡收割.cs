using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(30)][Serializable]
public class Card30 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        var gainHPSum = BothTurn.AttackAllEnemies(Atk).OfType<AttackResultHurt>().Sum(r => r.HurtDamage);
        BothTurn.GainCurHP(gainHPSum);
        return UniTask.CompletedTask;
    }
}