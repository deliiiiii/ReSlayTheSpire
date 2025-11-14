using System;
using System.Collections.Generic;

namespace RSTS;
[EnemyID(0)][Serializable]
internal class Enemy0(EnemyConfigMulti config) : EnemyDataBase(config)
{
    protected override IEnumerable<IntentionBase> IntentionSeq()
    {
        var attack0 = NthIntention(0);
        var attack1 = NthIntention(1);
        var attack2 = NthIntention(2);
        while (true)
        {
            yield return attack0;
            yield return attack1;
            yield return attack2;
        }
    }
}