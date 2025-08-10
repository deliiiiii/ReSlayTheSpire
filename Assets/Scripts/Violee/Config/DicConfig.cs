using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(DicConfig), menuName = "Violee/" + nameof(DicConfig))]
public class DicConfig : ScriptableObject
{
    public List<WordInfo> WordList = [];
}


[Serializable]
public class WordInfo
{
    public required string Word;
}
