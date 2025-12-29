using System;
using Cysharp.Threading.Tasks;
using RSTS.CDMV;

namespace RSTS;
[AttributeUsage(AttributeTargets.Class)]
public class CardAttribute(int id) : IDAttribute(id);

[Card(-1)][Serializable]
// ReSharper disable once InconsistentNaming
public class _Card_FallBack : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}