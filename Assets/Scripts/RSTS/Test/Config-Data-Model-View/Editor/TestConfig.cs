using System;
using UnityEditor;
using UnityEngine;

namespace RSTS.Test;

[Serializable]
[CreateAssetMenu(fileName = nameof(TestConfig), menuName = "RSTS/Test/" + nameof(TestConfig), order = 0)]
public class TestConfig : ConfigBase
{
    public int SpreadCount;
    public int SpreadMaxId;
    
    [SerializeReference]
    public required TestConfig2 Config2;
}
