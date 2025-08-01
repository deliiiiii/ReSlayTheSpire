using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;

public struct BoxPointDataNonSerialized
{
        
}
    
[Serializable]
public class BoxPointData : DataBase, IComparable
{
    public EBoxDir Dir;
    public Observable<int> CostWall = new (int.MaxValue / 2);
    // public Observable<int> CostStep;
    public Observable<bool> Visited = new (false);
    public Observable<bool> IsFlash = new (false);
        

        
    [NonSerialized] public List<BoxPointData> NextPointsInBox = [];
    [NonSerialized] public required BoxData BelongBox;
    [NonSerialized] public Vector3 Pos3D;
    [NonSerialized] public HashSet<WallData> WallSet = [];
    [NonSerialized] BingChaJi bingChaJi = new();

        
    #region Generate Map
    public void ResetBeforeDij()
    {
        CostWall.Value = int.MaxValue / 2;
        Visited.Value = false;

        WallSet = [];
        bingChaJi.Init(this);
    }
    public void AddWall(WallData wallData) => WallSet.Add(wallData);
    #endregion
        
        
    #region Connect and BCJ
    public void Merge(BoxPointData other) => bingChaJi.Merge(other.bingChaJi);
    public void VisitConnected()
    {
        foreach (var connectedB in bingChaJi.ConnectedSet)
        {
            connectedB.PointData.Visited.Value = true;
            foreach (var wallData in connectedB.PointData.WallSet)
            {
                wallData.Visited.Value = true;
            }
        }
    }
    public void FlashConnectedInverse()
    {
        foreach (var connectedB in bingChaJi.ConnectedSet)
        {
            connectedB.PointData.IsFlash.Value = !connectedB.PointData.IsFlash.Value;
        }
    }
        
    #endregion
        
        
    public int CompareTo(object obj)
    {
        if (!(obj is BoxPointData other))
            return 1;
        return this == other ? 0 : 1;
    }
}

[Serializable]
class BingChaJi
{
    [ShowInInspector] public int SetCount => Math.Max(con, Find().con);
    BingChaJi f = null!;
    public BoxPointData PointData = null!;
    HashSet<BingChaJi> connectedSet = null!;

    public HashSet<BingChaJi> ConnectedSet
    {
        get
        {
            var find = Find();
            return con >= find.con ? connectedSet : find.connectedSet;
        }
    }
    int con => connectedSet.Count;

    public void Init(BoxPointData pointData)
    {
        f = this;
        PointData = pointData;
        connectedSet = [this];
    }

    public void Merge(BingChaJi other)
    {
        var thisF = Find();
        var otherF = other.Find();
        if (thisF == otherF)
            return;
        thisF.connectedSet.AddRange(otherF.connectedSet);
        otherF.connectedSet = thisF.connectedSet;
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