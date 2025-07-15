using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class MapData : KeyedCollection<Vector2Int, BoxData>
    {
        protected override Vector2Int GetKeyForItem(BoxData item)
        {
            return item.Pos;
        }
    }
}