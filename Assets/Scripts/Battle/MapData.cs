using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MapData
{
    public List<MapNodeData> MapNodes;

    public static MapData GetDefault()
    {
        return new MapData
        {
            MapNodes = new List<MapNodeData>
            {
                new()
                {
                    NodeID = 0,
                    MapNodeType = MapNodeType.NormalEnemy,
                    Pos = new Vector2(0, 0),
                    NextNodes = new List<int> { 1, 2, 3 },
                },
            },
        };
    }
}