using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5)][Serializable]
public class Card5 : Card
{
    int Block => BlockAt(0);
    int Atk => AtkAt(1);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.GainBlock(Block);
        bothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}