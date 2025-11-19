using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(103)][Serializable]
public class Card103 : CardInTurn
{
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        var ret = bothTurnData.TryPullOneFromDraw(shouldRefill: false, out var toYieldExhaust);
        if (!ret)
        {
            MyDebug.Log("破灭：抽牌堆无牌可抽");
            return;
        }

        MyDebug.Log($"破灭：{toYieldExhaust.Parent.Config.name}");
        await bothTurnData.YieldBlindAsync(toYieldExhaust, [new YieldModifyForceExhaust()]);
    }
}