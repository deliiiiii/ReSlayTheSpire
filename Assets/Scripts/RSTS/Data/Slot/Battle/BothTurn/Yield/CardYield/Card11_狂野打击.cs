using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(11)][Serializable]
public class Card11 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddTempToDraw(CreateNowhereCard(5004, BothTurn));
        
        return UniTask.CompletedTask;
    }
}
