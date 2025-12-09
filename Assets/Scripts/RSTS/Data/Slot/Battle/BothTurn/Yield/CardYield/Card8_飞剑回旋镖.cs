using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(8)][Serializable]
public class Card8 : CardInTurn
{
    int Atk => AtkAt(0);
    int AtkTime => AtkTimeAt(1);
    public override async UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        await BothTurn.AttackEnemyRandomlyMultiTimesAsync(Atk, AtkTime);
    }
}
