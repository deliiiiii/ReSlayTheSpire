using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(20)][Serializable]
public class Card20 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddTempToDraw(Create(5003));
        
        return UniTask.CompletedTask;
    }
}
