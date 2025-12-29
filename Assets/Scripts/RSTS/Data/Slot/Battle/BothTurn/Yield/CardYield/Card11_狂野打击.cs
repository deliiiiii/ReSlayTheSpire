using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(11)][Serializable]
public class Card11 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddTempToDraw(Create(5004));
        
        return UniTask.CompletedTask;
    }
}
