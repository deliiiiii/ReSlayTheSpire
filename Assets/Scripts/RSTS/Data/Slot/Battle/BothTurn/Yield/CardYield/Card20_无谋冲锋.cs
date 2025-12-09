using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(20)][Serializable]
public class Card20 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddTempToDraw(CreateNowhereCard(5003, BothTurn));
        
        return UniTask.CompletedTask;
    }
}
