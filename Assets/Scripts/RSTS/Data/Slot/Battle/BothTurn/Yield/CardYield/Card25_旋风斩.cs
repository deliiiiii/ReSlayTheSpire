using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(25)][Serializable]
public class Card25 : Card
{
    int Atk => AtkAt(0);

    public override async UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        await bothTurn.AttackAllEnemiesMultiTimesAsync(Atk, cost);
    }

}