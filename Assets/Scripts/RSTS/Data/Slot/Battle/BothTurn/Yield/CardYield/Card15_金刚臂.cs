using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(15)][Serializable]
public class Card15 : CardInTurn
{
    int Atk => AtkAt(0);
    BuffDataWeak WeakBuff => BuffAt<BuffDataWeak>(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddBuffToEnemy(target, WeakBuff);
        return UniTask.CompletedTask;
    }
}