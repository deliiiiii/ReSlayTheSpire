using System;
namespace RSTS;
[EnemyID(2)][Serializable]
public class Enemy2 : EnemyDataBase
{
    protected override IntentionBase? CurIntention()
    {
        return NthIntention(0);
    }
}