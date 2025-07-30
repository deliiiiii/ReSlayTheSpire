using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class BoxPointData : DataBase
    {
        public EBoxDir Dir;
        public Observable<int> CostWall = new (int.MaxValue / 2);
        // public Observable<int> CostStep;
        public Observable<bool> Visited = new (false);
        public Observable<bool> IsFlash = new (false);
        

        [NonSerialized] public List<BoxPointData> NextPointsInBox = [];
        [NonSerialized] public required BoxData BelongBox;
        [NonSerialized] public Vector3 Pos3D;
        [NonSerialized][ShowInInspector] BingChaJi bingChaJi = new();

        public void ResetBeforeDij()
        {
            CostWall.Value = int.MaxValue / 2;
            Visited.Value = false;
            bingChaJi.Init(this);
        }

        public void Merge(BoxPointData other) => bingChaJi.Merge(other.bingChaJi);

        public void VisitConnected()
        {
            foreach (var connectedB in bingChaJi.ConnectedSet)
            {
                connectedB.PointData.Visited.Value = true;
            }
            
        }
        public void FlashConnectedInverse()
        {
            foreach (var connectedB in bingChaJi.ConnectedSet)
            {
                connectedB.PointData.IsFlash.Value = !connectedB.PointData.IsFlash.Value;
            }
        }
    }
}