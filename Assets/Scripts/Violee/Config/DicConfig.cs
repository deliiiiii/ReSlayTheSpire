using System;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(DicConfig), menuName = "Violee/" + nameof(DicConfig))]
public class DicConfig : ScriptableObject
{
    public SerializableDictionary<string, WordInfo> Dic = [];
}


[Serializable]
public class WordInfo
{
    
}
