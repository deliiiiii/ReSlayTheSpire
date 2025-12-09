using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(14)][Serializable]
public class Card14 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackAllEnemies(Atk);
        return UniTask.CompletedTask;
    }
}
