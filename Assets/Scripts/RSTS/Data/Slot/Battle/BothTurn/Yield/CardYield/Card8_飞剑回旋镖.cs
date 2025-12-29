using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(8)][Serializable]
public class Card8 : Card
{
    int Atk => AtkAt(0);
    int AtkTime => AtkTimeAt(1);
    public override async UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        await bothTurn.AttackEnemyRandomlyMultiTimesAsync(Atk, AtkTime);
    }
}
