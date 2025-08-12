using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;


public static class BoxPointDataExt
{
    public static IEnumerable<BoxPointData> AtWallGetInsidePoints(this BoxPointData curPoint, WallData wallData)
    {
        EBoxDir? nextPointDir = (curPoint.Dir, wallData.WallType) switch
        {
            (EBoxDir.Up, EWallType.S1 or EWallType.S4) => EBoxDir.Down,
            (EBoxDir.Right, EWallType.S2 or EWallType.S8) => EBoxDir.Left,
            (EBoxDir.Down, EWallType.S4 or EWallType.S1) => EBoxDir.Up,
            (EBoxDir.Left, EWallType.S8 or EWallType.S2) => EBoxDir.Right,

            (EBoxDir.Up, EWallType.T1248) => EBoxDir.Right,
            (EBoxDir.Up, EWallType.T2481) => EBoxDir.Left,

            (EBoxDir.Right, EWallType.T1248) => EBoxDir.Up,
            (EBoxDir.Right, EWallType.T2481) => EBoxDir.Down,

            (EBoxDir.Down, EWallType.T1248) => EBoxDir.Left,
            (EBoxDir.Down, EWallType.T2481) => EBoxDir.Right,

            (EBoxDir.Left, EWallType.T1248) => EBoxDir.Down,
            (EBoxDir.Left, EWallType.T2481) => EBoxDir.Up,

            _ => null,
        };
        
        // MyDebug.Log($"Inside {curPoint.BelongBox.Pos2D} {curPoint.Dir} {wallData.WallType} {nextPointDir}");
    
        if (nextPointDir == null)
            return [];
        return curPoint.NextPointAndWallSet
            .FirstOrDefault(pair => pair.Item1.Dir == nextPointDir)
            .Item1?.BingChaJi.ConnectedSet ?? [];
    }
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
    [SerializeReference] public HashSet<(BoxPointData, WallData)> NextPointAndWallSet = [];
    [SerializeReference] public BingChaJi<BoxPointData> BingChaJi = new();

        
    #region Generate Map
    public void ResetBeforeDij()
    {
        CostWall.Value = int.MaxValue / 2;
        Visited.Value = false;

        NextPointAndWallSet = [];
        BingChaJi.Init(this);
    }
    public void AddWallAndNextPoint((BoxPointData, WallData) pair) => NextPointAndWallSet.Add(pair);
    #endregion
        
        
    #region Connect and BCJ
    public void Merge(BoxPointData other) => BingChaJi.Merge(other.BingChaJi);
    public void VisitConnected()
    {
        foreach (var connectedPoint in BingChaJi.ConnectedSet)
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
    //     foreach (var connectedPoint in BingChaJi.ConnectedSet)
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

    // // 玩家可能站在很远的地方。。。所以先SelectMany拿到这个房间所有的点
    // public IEnumerable<BoxPointData> AtWallGetInsidePoints(WallData wallData)
    // {
    //     var ret = BelongBox.PointDataMyDic.Values
    //         .SelectMany(nextP => nextP.NextPointAndWallSet)
    //         .FirstOrDefault(pair => pair.Item2 == wallData)
    //         .Item1?.BingChaJi.ConnectedSet ?? [];
    //     return ret;
    // }

    

    public IEnumerable<SceneItemData> ConnectedPointItems()
    {
        // return BingChaJi.ConnectedSet
        //     .Any(p => p.BelongBox.SceneItemDataMyList
        //         .Any(s => s is RecordPlayerItemData && s.OccupyFloorSet.Contains(p.Dir)));

        return BingChaJi.ConnectedSet.SelectMany(
            p => p.BelongBox.SceneItemDataMyList
                .Where(s => s.OccupyFloorSet.Contains(p.Dir) || s.OccupyAirSet.Contains(p.Dir)));
    }
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
public class BingChaJi<T>
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