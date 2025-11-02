using System;

namespace RSTS;
[EnemyID(0)][Serializable]
public class Enemy0 : EnemyDataBase
{
    protected override IntentionBase? CurIntention()
    {
        return NthIntention(0);
    }
}