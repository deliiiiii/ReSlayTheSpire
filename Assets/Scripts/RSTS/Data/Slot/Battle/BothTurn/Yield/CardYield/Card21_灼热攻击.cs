using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(21)][Serializable]
public class Card21 : CardInTurn
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}