using System;
namespace RSTS;
[EnemyID(1)][Serializable]
public class Enemy1 : EnemyDataBase
{
    protected override IntentionBase CurIntention()
    {
        return NthIntention(0);
    }
}