using System;
using JetBrains.Annotations;
using RSTS.CDMV;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

[CreateAssetMenu(menuName = "RSTS/Bottle", order = 1)][PublicAPI][Serializable]
public class BottleConfig : ConfigMulti<BottleConfig>
{
    protected override string PrefixName => "Bottle";
}