using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class MapData
    {
        public SerializableDictionary<Vector2Int, BoxData> BoxDic;
    }
}