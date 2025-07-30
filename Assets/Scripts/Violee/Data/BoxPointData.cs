using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
        [NonSerialized] HashSet<WallData> wallSet = [];
        [NonSerialized][ShowInInspector] BingChaJi bingChaJi = new();

        public void ResetBeforeDij()
        {
            CostWall.Value = int.MaxValue / 2;
            Visited.Value = false;

            wallSet = [];
            bingChaJi.Init(this);
        }

        public void Merge(BoxPointData other) => bingChaJi.Merge(other.bingChaJi);
        public void AddWall(WallData wallData) => wallSet.Add(wallData);

        public void VisitConnected()
        {
            foreach (var connectedB in bingChaJi.ConnectedSet)
            {
                connectedB.PointData.Visited.Value = true;
            }

            foreach (var wallData in wallSet)
            {
                wallData.Visited.Value = true;
            }
            
        }
        public void FlashConnectedInverse()
        {
            foreach (var connectedB in bingChaJi.ConnectedSet)
            {
                connectedB.PointData.IsFlash.Value = !connectedB.PointData.IsFlash.Value;
            }
        }
        
        [Serializable]
        class BingChaJi
        {
            [ShowInInspector] public int SetCount => Math.Max(con, Find().con);
            BingChaJi f = null!;
            public BoxPointData PointData = null!;
            public HashSet<BingChaJi> ConnectedSet = null!;
            int con => ConnectedSet.Count;

            public void Init(BoxPointData pointData)
            {
                f = this;
                PointData = pointData;
                ConnectedSet = [this];
            }
    
            public void Merge(BingChaJi other)
            {
                var thisF = Find();
                var otherF = other.Find();
                if (thisF == otherF)
                    return;
                ConnectedSet.AddRange(otherF.ConnectedSet);
                otherF.ConnectedSet = ConnectedSet;
                // MyDebug.LogWarning(
                //     $"CurPoint {pointData.BelongBox.Pos2D}.{pointData.Dir} OtherPoint {other.pointData.BelongBox.Pos2D}.{other.pointData.Dir}");
                // MyDebug.LogWarning($"Merge {thisF.pointData.BelongBox.Pos2D}.{thisF.pointData.Dir} {thisF.Con} , {otherF.pointData.BelongBox.Pos2D}.{otherF.pointData.Dir} {otherF.Con} {GetHashCode() > otherF.GetHashCode()}");
                if (thisF.GetHashCode() > otherF.GetHashCode())
                {
                    otherF.f = thisF;
                }
                else
                {
                    thisF.f = otherF;
                }
            }

            BingChaJi Find()
            {
                return f == this ? f : f = f.Find();
            }
        }

    }
}