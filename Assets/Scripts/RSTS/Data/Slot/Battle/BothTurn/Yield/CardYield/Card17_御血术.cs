using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(17)][Serializable]
public class Card17 : Card
{
    int LoseHP => MiscAt(0);
    int Atk => AtkAt(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.LoseHP(LoseHP);
        bothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}