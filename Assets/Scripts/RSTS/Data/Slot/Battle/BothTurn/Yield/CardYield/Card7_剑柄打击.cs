using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(7)][Serializable]
public class Card7 : CardInTurn
{
    int Atk => AtkAt(0);
    int Draw => DrawAt(1);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.DrawSome(Draw);
        return UniTask.CompletedTask;
    }
}
