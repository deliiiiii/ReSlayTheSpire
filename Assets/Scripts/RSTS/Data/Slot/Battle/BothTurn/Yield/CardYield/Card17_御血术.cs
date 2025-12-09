using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(17)][Serializable]
public class Card17 : CardInTurn
{
    int LoseHP => MiscAt(0);
    int Atk => AtkAt(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.LoseHP(LoseHP);
        BothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}