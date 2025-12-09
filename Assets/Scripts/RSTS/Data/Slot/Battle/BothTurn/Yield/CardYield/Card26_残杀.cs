using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(26)][Serializable]
public class Card26 : CardInTurn
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}