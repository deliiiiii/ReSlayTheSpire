using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RSTS;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
[CreateAssetMenu(menuName = "RSTS/Enemy",fileName = nameof(EnemyConfigMulti), order = 4)][PublicAPI][Serializable]
public class EnemyConfigMulti : ConfigMulti<EnemyConfigMulti>
{
    protected override string PrefixName => "Enemy";
}