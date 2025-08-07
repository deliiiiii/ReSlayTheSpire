using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;
    
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
    [SerializeReference] public HashSet<(BoxPointData, WallData)> NextPointAndWallSet = [];
    [SerializeReference] BingChaJi<BoxPointData> bingChaJi = new();

        
    #region Generate Map
    public void ResetBeforeDij()
    {
        CostWall.Value = int.MaxValue / 2;
        Visited.Value = false;

        NextPointAndWallSet = [];
        bingChaJi.Init(this);
    }
    public void AddWallAndNextPoint((BoxPointData, WallData) pair) => NextPointAndWallSet.Add(pair);
    #endregion
        
        
    #region Connect and BCJ
    public void Merge(BoxPointData other) => bingChaJi.Merge(other.bingChaJi);
    public void VisitConnected()
    {
        foreach (var connectedPoint in bingChaJi.ConnectedSet)
        {
            connectedPoint.Visit();
        }
    }
    public void Visit()
    {
        Visited.Value = true;
        foreach (var pair in NextPointAndWallSet)
        {
            pair.Item2.Visited.Value = true;
        }
    }
    
    // public void FlashConnected(bool enable)
    // {
    //     foreach (var connectedPoint in bingChaJi.ConnectedSet)
    //     {
    //         connectedPoint.IsFlash.Value = enable;
    //     }
    // }
    // x => x?.FlashConnectedInverse(), x => x?.FlashConnectedInverse()
    public void Flash(bool enable)
    {
        IsFlash.Value = enable;
    }

    public IEnumerable<WallData> InvalidWalls() 
        => NextPointAndWallSet
            .Where(pair => pair.Item1.CostWall - CostWall >= BoxData.WallCost)
            .Select(x => x.Item2);

    // 玩家可能站在很远的地方。。。所以先SelectMany拿到这个房间所有的点
    public IEnumerable<BoxPointData> AtWallGetInsidePoints(WallData wallData)
    {
        var ret = BelongBox.PointDataMyDic.Values
            .SelectMany(nextP => nextP.NextPointAndWallSet)
            .FirstOrDefault(pair => pair.Item2 == wallData)
            .Item1?.bingChaJi.ConnectedSet ?? [];
        return ret;
    }
    
        // => 
        // => NextPointAndWallSet.FirstOrDefault(pair => pair.Item2 == wallData).Item1?.bingChaJi.ConnectedSet ?? [];

    #endregion


    public bool HasSWall()
    {
        return BelongBox.HasSWallByDir(Dir, out _) || 
               NextPointAndWallSet.Any(pair => pair.Item2.WallType == BoxHelper.WallDirToType(BoxHelper.OppositeDirDic[Dir]));
    }
        
    public int CompareTo(object obj)
    {
        if (obj is not BoxPointData other)
            return 1;
        return this == other ? 0 : 1;
    }
}

[Serializable]
class BingChaJi<T>
{
    [ShowInInspector] public int SetCount => Math.Max(con, Find().con);
    [SerializeField] BingChaJi<T> f = null!;
    HashSet<T> connectedSet = [];

    public HashSet<T> ConnectedSet
    {
        get
        {
            var find = Find();
            return con >= find.con ? connectedSet : find.connectedSet;
        }
    }
    int con => connectedSet.Count;

    public void Init(T belongTo)
    {
        f = this;
        connectedSet = [belongTo];
    }

    public void Merge(BingChaJi<T> other)
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

    BingChaJi<T> Find()
    {
        return f == this ? f : f = f.Find();
    }
}