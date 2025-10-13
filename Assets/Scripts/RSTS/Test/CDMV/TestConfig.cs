using System;
using RSTS.CDMV;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS.Test;

[Serializable]
[CreateAssetMenu(fileName = nameof(TestConfig), menuName = "RSTS/Test/" + nameof(TestConfig), order = 0)]
public class TestConfig : ConfigBase
{
    [field: SerializeField]
    public int SpreadCount { get; protected set; }
    [field: SerializeField]
    public int SpreadMaxId { get; protected set; }
    
    [field: SerializeReference]
    public TestConfig2 Config2 { get;set; }
}
