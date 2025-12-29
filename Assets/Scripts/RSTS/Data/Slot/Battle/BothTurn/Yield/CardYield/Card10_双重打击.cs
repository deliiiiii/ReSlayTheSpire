using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(10)][Serializable]
public class Card10 : Card
{
    int Atk => AtkAt(0);
    public override async UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        await bothTurn.AttackEnemyMultiTimesAsync(target, Atk, 2);
    }
}
