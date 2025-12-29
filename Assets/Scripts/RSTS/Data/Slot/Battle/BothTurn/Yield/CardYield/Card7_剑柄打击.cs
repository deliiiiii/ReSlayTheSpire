using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(7)][Serializable]
public class Card7 : Card
{
    int Atk => AtkAt(0);
    int Draw => DrawAt(1);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.DrawSome(Draw);
        return UniTask.CompletedTask;
    }
}
