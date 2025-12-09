using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5)][Serializable]
public class Card5 : CardInTurn
{
    int Block => BlockAt(0);
    int Atk => AtkAt(1);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.GainBlock(Block);
        BothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}