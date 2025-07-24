using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class BoxPointData
    {
        public EBoxDir Dir;
        public Observable<int> CostWall = new (int.MaxValue / 2);
        // public Observable<int> CostStep;
        public Observable<bool> Visited = new (false);


        [NonSerialized] public List<BoxPointData> NextPointsInBox = [];
        [NonSerialized] public required BoxData BelongBox;
        [NonSerialized] public Vector3 Pos3D;
        public void UpdateNextPointCost()
        {
            foreach (var nextPoint in NextPointsInBox)
            {
                nextPoint.CostWall.Value = Math.Min(
                    nextPoint.CostWall.Value,
                    CostWall + BelongBox.CostTilt(Dir, nextPoint.Dir));
            }
        }
    }
}