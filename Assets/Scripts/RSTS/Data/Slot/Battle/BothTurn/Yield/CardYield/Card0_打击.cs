using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(0)][Serializable]
public class Card0 : Card
{
    int Atk => AtkAt(0);
    
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}