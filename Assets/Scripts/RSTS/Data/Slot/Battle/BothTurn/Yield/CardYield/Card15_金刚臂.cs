using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(15)][Serializable]
public class Card15 : Card
{
    int Atk => AtkAt(0);
    BuffDataWeak WeakBuff => BuffAt<BuffDataWeak>(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddBuffToEnemy(target, WeakBuff);
        return UniTask.CompletedTask;
    }
}