
using System;

namespace RSTS;

[Serializable]
public class ItemData
{
    public ItemConfigMulti ItemConfig;
    public ItemCountData CountData;

    public ItemData(ItemConfigMulti itemConfig)
    {
        ItemConfig = itemConfig;
        CountData = itemConfig.CountConfig switch
        {
            ItemCountAscentConfig ascentConfig => new ItemCountAscent { CurCount = 0 },
            ItemCountNoneConfig or _ => new ItemCountNone(),
        };
    }
}


public abstract class ItemCountData;

[Serializable]
public class ItemCountAscent : ItemCountData
{
    public int CurCount;
}

[Serializable]
public class ItemCountNone : ItemCountData;