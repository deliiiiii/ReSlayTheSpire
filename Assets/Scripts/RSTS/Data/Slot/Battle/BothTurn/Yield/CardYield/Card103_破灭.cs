using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(103)][Serializable]
public class Card103 : Card
{
    public override async UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        var ret = bothTurn.TryPullOneFromDraw(shouldRefill: false, out var toYieldExhaust);
        if (!ret)
        {
            MyDebug.Log("破灭：抽牌堆无牌可抽");
            return;
        }

        MyDebug.Log($"破灭：{toYieldExhaust.Config.name}");
        await bothTurn.YieldHandAsync(toYieldExhaust, null, [new YieldModifyFromDraw(), new YieldModifyForceExhaust()]);
    }
}