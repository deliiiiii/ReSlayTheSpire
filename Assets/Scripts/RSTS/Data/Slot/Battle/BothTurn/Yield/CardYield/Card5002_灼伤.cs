using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5002)][Serializable]
public class Card5002 : Card
{
    int Atk => MiscAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
    public override void OnExitPlayerYieldCard(YieldCard yieldCard)
    {
        yieldCard.BelongFSM.BurnPlayer(Atk);
    }
}
