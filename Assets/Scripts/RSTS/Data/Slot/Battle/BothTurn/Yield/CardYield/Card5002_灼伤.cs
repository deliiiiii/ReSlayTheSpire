using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5002)][Serializable]
public class Card5002 : CardInTurn
{
    int Atk => MiscAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
    public override void OnExitPlayerYieldCard()
    {
        BothTurn.BurnPlayer(Atk);
    }
}
