using System;
using RSTS.CDMV;
using UnityEngine;
namespace RSTS;
[CreateAssetMenu(menuName = "RSTS/Item", fileName = "ItemConfig", order = 2)]
public class ItemConfigMulti : ConfigMulti<ItemConfigMulti>
{ 
    protected override string PrefixName => "Item";
    [SerializeReference]
    public ItemCountConfig CountConfig;
}

public abstract class ItemCountConfig;

[Serializable]
public class ItemCountAscentConfig : ItemCountConfig
{
    public int MaxCount;
}
[Serializable]
public class ItemCountNoneConfig : ItemCountConfig;