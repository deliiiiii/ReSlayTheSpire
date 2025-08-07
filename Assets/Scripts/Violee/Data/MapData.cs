using System;
using UnityEngine;

namespace Violee;

[Serializable]
public class MapData
{
    public MyDictionary<Vector2Int, BoxData> BoxDataDic = [];
    public DateTime DateTime;
}