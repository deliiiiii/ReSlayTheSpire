using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "TestS", menuName = "Violee/TestS", order = 0)]
public class TestS : SerializedScriptableObject
{
    [OdinSerialize]
    public Dictionary<int, string> Dic = [];
    
    [OdinSerialize][NonSerialized]
    public List<int> List = [];
}