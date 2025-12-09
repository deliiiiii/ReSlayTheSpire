using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(25)][Serializable]
public class Card25 : CardInTurn
{
    int Atk => AtkAt(0);

    public override async UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        await BothTurn.AttackAllEnemiesMultiTimesAsync(Atk, cost);
    }

}