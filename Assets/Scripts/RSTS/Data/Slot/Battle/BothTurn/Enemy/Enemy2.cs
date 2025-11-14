using System;
using System.Collections.Generic;

namespace RSTS;
[EnemyID(2)][Serializable]
internal class Enemy2(EnemyConfigMulti config) : EnemyDataBase(config)
{
    protected override IEnumerable<IntentionBase> IntentionSeq()
    {
        if (Config.IntentionList.Count == 0)
            yield break;
        var i0 = NthIntention(0);
        while (true)
            yield return i0;
    }
}