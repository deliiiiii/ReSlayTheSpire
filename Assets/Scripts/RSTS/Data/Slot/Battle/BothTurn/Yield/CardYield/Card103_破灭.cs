using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(103)][Serializable]
public class Card103 : CardInTurn
{
    public override async UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        var ret = BothTurn.TryPullOneFromDraw(shouldRefill: false, out var toYieldExhaust);
        if (!ret)
        {
            MyDebug.Log("破灭：抽牌堆无牌可抽");
            return;
        }

        MyDebug.Log($"破灭：{toYieldExhaust.Config.name}");
        await BothTurn.YieldHandAsync(toYieldExhaust, null, [new YieldModifyFromDraw(), new YieldModifyForceExhaust()]);
    }
}