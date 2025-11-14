using System;
using System.Collections.Generic;

namespace RSTS;
[EnemyID(1)][Serializable]
internal class Enemy1(EnemyConfigMulti config) : EnemyDataBase(config)
{
    protected override IEnumerable<IntentionBase> IntentionSeq()
    {
        var seed = new Random(114514);
        var giveWeak = NthIntention(0);
        var attack = NthIntention(1);
        List<IntentionBase> list = [giveWeak, attack];
        while (true)
        {
            yield return list.RandomItem(weightList: [1, 2], seed: seed);
            // yield return giveWeak;
        }
    }
}