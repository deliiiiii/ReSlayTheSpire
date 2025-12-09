using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(10)][Serializable]
public class Card10 : CardInTurn
{
    int Atk => AtkAt(0);
    public override async UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        await BothTurn.AttackEnemyMultiTimesAsync(target, Atk, 2);
    }
}
