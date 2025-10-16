using System;
using RSTS.CDMV;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;
[CreateAssetMenu(menuName = "RSTS/Item", fileName = "Item", order = 4)]
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