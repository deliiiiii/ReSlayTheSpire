using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;

public class GenerateStreamParam(
    MyKeyedCollection<Vector2Int, BoxData> boxKList, 
    HashSet<Vector2Int> emptyPosSet,
    HashSet<WallData> edgeWallSet)
{
    public readonly MyKeyedCollection<Vector2Int, BoxData> BoxKList = boxKList;
    public readonly HashSet<Vector2Int> EmptyPosSet = emptyPosSet;
    public readonly HashSet<WallData> EdgeWallSet = edgeWallSet;
}



internal class MapManager : SingletonCS<MapManager>
{
    static readonly MapModel mapModel;
    static readonly ObjectPool<BoxModel> boxPool;
    static MapManager()
    {
        mapModel = Configer.MapModel;
        boxPool = new ObjectPool<BoxModel>(Configer.BoxModel, Instance.go.transform, 42);
        
        GenerateStream = Instance.Bind(() => new GenerateStreamParam(boxKList, [], [])) 
            .ToStreamAsync(StartGenerate);
        // (t1,t2)
        DijkstraStream = Instance.Bind(() => GenerateStream.Result.Value)
            .WithA(() => BoxHelper.Pos2DTo3DPoint(StartPos, StartDir))
            .ToStreamAsync(Dijkstra)
            .OnEnd(param => VisitEdgeWalls(param.Item1.EdgeWallSet));
        
        GenerateStream.EndWith(DijkstraStream);
        
        Binder.Update(_ =>
        {
            if (Input.GetKeyDown(KeyCode.R))
                Task.FromResult(GenerateStream.CallTriggerAsync());
        }, EUpdatePri.Input);

    }
    
    
    public static float MaxSize => Mathf.Max(Width, Height) * BoxHelper.BoxSize;
    
    static int Height => mapModel.Height;
    static int Width => mapModel.Width;
    static Vector2Int StartPos => mapModel.StartPos;
    static EBoxDir StartDir => mapModel.StartDir;
    static readonly MyKeyedCollection<Vector2Int, BoxData> boxKList = new (b => b.Pos2D);
    static bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    static bool HasBox(Vector2Int pos) => boxKList.Contains(pos);
    
    

    #region Visit
    static readonly Observable<BoxPointData> playerCurPoint = new(null!, 
        x => x?.FlashConnectedInverse(), x => x?.FlashConnectedInverse());
    public static void TickPlayerVisit(Vector3 playerPos)
    {
        var x = playerPos.x;
        var z = playerPos.z;
        var boxPos2D = BoxHelper.Pos3DTo2D(playerPos);
        var boxPos3D = BoxHelper.Pos2DTo3DBox(boxPos2D);
        if (!HasBox(boxPos2D))
        {
            MyDebug.LogWarning($"Why !HasBox({boxPos3D}) PlayerPos:{playerPos}");
            return;
        }

        foreach (var dir in BoxHelper.AllBoxDirs)
        {
            var edgeCenterPos = BoxHelper.Pos2DTo3DEdge(boxPos2D, dir);
            var edgeX = edgeCenterPos.x;
            var edgeZ = edgeCenterPos.z;
            var pointData = boxKList[boxPos2D].PointKList[dir];
            // MyDebug.Log($"dir:{dir} x:{x} edgeX:{edgeX} z:{z} edgeZ:{edgeZ}");
            if (Math.Abs(x - edgeX) + Math.Abs(z - edgeZ) <= BoxHelper.BoxSize * Configer.BoxConfigList.WalkInTolerance)
            {
                playerCurPoint.Value = pointData;
                if(!playerCurPoint.Value.Visited)
                {
                    playerCurPoint.Value.VisitConnected();
                    MyDebug.Log($"First Enter Point!!{boxPos2D}:{dir}");
                }
            }
        }
    }

    static void VisitEdgeWalls(HashSet<WallData> edgeWallSet)
    {
        edgeWallSet.ForEach(wallData => wallData.Visited.Value = true);
    }
    #endregion
    
    
    #region Generate

    public static readonly Stream<GenerateStreamParam> GenerateStream;

    public static readonly Stream<(GenerateStreamParam, Vector3)> DijkstraStream;
    static async Task StartGenerate(GenerateStreamParam param)
    {
        var fBoxKList = param.BoxKList;
        var fEmptyPosSet = param.EmptyPosSet;
        var fEdgeWallSet = param.EdgeWallSet;
        void RemoveAllBoxes()
        {
            fBoxKList.ForEach(DestroyBox);
            fBoxKList.Clear();
            fEmptyPosSet.Clear();
            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width; i++)
                {
                    fEmptyPosSet.Add(new Vector2Int(i, j));
                }
            }
        }
        try
        {
            RemoveAllBoxes();
            await GenerateOneFakeConnection(true, fEmptyPosSet, fEdgeWallSet);
            while (fEmptyPosSet.Count > 0)
            {
                await GenerateOneFakeConnection(false, fEmptyPosSet, fEdgeWallSet);
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    static async Task GenerateOneFakeConnection(bool startWithStartLoc, HashSet<Vector2Int> fEmptyPosSet,HashSet<WallData> edgeWallSet)
    {
        async Task<BoxData> AddBoxAsync(Vector2Int pos, BoxConfig config)
        {
            await Configer.SettingsConfig.YieldFrames();
            var boxData = new BoxData(pos, config);
            MyDebug.Log($"Add box {config.Walls} at {pos}");
            boxKList.Add(boxData);
            fEmptyPosSet.Remove(pos);
            await SpawnBox3D(boxData);
            return boxData;
        }
        // void RemoveBox(BoxData boxData)
        // {
        //     boxKList.Remove(boxData);
        //     fEmptyPosSet.Add(boxData.Pos2D);
        //     DestroyBox(boxData);
        // }
        
        try
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : fEmptyPosSet.First();
            var firstBox = await AddBoxAsync(firstLoc, BoxHelper.EmptyBoxConfig);
            edgeBoxStack.Push(firstBox);
            while (edgeBoxStack.Count > 0)
            {
                var curBox = edgeBoxStack.Pop();
                var nextPairs = BoxHelper.GetNextLocAndGoInDirList(curBox.Pos2D);
                foreach (var (nextPos, nextGoInDir) in nextPairs)
                {
                    // “下一格”
                    var curGoOutDir = BoxHelper.OppositeDirDic[nextGoInDir];
                    if (!InMap(nextPos))
                    {
                        var edgeWall = new WallData(curGoOutDir, EDoorType.None);
                        edgeWallSet.Add(edgeWall);
                        curBox.AddSWall(edgeWall);
                        // MyDebug.Log($"ReachMapEdge, AddWall {curBox.Pos}:{curGoOutDir}");
                        continue;
                    }
                    if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir))
                    {
                        var boxConfig = 
                            Configer.BoxConfigList.BoxConfigs.RandomItemWeighted(
                                x => !BoxHelper.HasSWallByByteAndDir(x.Walls, nextGoInDir),
                                x => x.BasicWeight);
                        var nextBox = await AddBoxAsync(nextPos, boxConfig);
                        var nextNextPairs = BoxHelper.GetNextLocAndGoInDirList(nextPos);
                        foreach (var (nextNextPos, nextNextGoInDir) in nextNextPairs)
                        {
                            // nextNextGoInDir:  “下一格”的相邻格的走入方向
                            var nextGoOutDir = BoxHelper.OppositeDirDic[nextNextGoInDir];
                            if (InMap(nextNextPos) && HasBox(nextNextPos))
                            {
                                var nextNextBox = boxKList[nextNextPos];
                                if (nextNextBox.HasSWallByDir(nextNextGoInDir))
                                {
                                    nextBox.RemoveSWall(nextGoOutDir);
                                    // MyDebug.Log($"WallRepeat, RemoveWall {nextBox.Pos}:{nextGoOutDir}");
                                }
                            }
                        }
                        
                        edgeBoxStack.Push(nextBox);
                    }
                }
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    static async Task Dijkstra((GenerateStreamParam, Vector3) pair)
    {
        try
        {
            var fBoxKList = pair.Item1.BoxKList;
            fBoxKList.ForEach(boxData => boxData.ResetBeforeDij());
            var vSet = new HashSet<BoxPointData>();
            var pq = new SimplePriorityQueue<BoxPointData, int>();
            var startBox = fBoxKList[StartPos];
            var startPoint = startBox.PointKList[StartDir];
            startPoint.CostWall.Value = 0;
            pq.Enqueue(startPoint, 0);
            while (pq.Count != 0)
            {
                var curPoint = pq.Dequeue();
                vSet.Add(curPoint);
                var curCost = curPoint.CostWall;
                var curBox = curPoint.BelongBox;
                var curDir = curPoint.Dir;
                var nextPos = BoxHelper.NextPos(curBox.Pos2D, curDir);
                if (InMap(nextPos))
                {
                    var nextBox = fBoxKList[nextPos];
                    var oppositeDir = BoxHelper.OppositeDirDic[curDir];
                    var nextPoint = nextBox.PointKList[oppositeDir];
                    var costSWall = curBox.CostStraight(curDir, out var wallData1) + nextBox.CostStraight(oppositeDir, out var wallData2);
                    nextPoint.CostWall.Value = Math.Min(
                        nextPoint.CostWall.Value,
                        curCost.Value + costSWall);
                    if (wallData1 != null)
                        curPoint.AddWall(wallData1);
                    if (wallData2 != null)
                        curPoint.AddWall(wallData2);
                    
                    if (!vSet.Contains(nextPoint))
                    {
                        if(pq.Contains(nextPoint))
                            pq.UpdatePriority(nextPoint, nextPoint.CostWall);
                        else
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                        if(costSWall == 0)
                        {
                            curPoint.Merge(nextPoint);
                        }
                    }
                }
                curPoint.NextPointsInBox
                    .ForEach(nextPoint =>
                    {
                        curBox.CostTilt(curPoint.Dir, nextPoint.Dir, out var wallData3);
                        if(wallData3 != null)
                            curPoint.AddWall(wallData3);
                    });
                curPoint.NextPointsInBox
                    .Where(nextPoint => !vSet.Contains(nextPoint))
                    .ForEach(nextPoint =>
                    {
                        var costTWall = curBox.CostTilt(curPoint.Dir, nextPoint.Dir, out _);
                        nextPoint.CostWall.Value = Math.Min(
                            nextPoint.CostWall.Value,
                            curPoint.CostWall + costTWall);
                        if (pq.Contains(nextPoint))
                            pq.UpdatePriority(nextPoint, nextPoint.CostWall);
                        else
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                        if(costTWall == 0)
                            curPoint.Merge(nextPoint);
                    });
                await Configer.SettingsConfig.YieldFrames();
            }
            MyDebug.Log("Dijkstra finished!");
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    static readonly MyKeyedCollection<Vector3, BoxModel> boxModel3DDic = new(b => b.transform.position);
    static async Task SpawnBox3D(BoxData fBoxData)
    {
        var boxModel = await boxPool.MyInstantiate();
        boxModel.ReadData(fBoxData);
        boxModel3DDic.Add(boxModel);
    }
        
    static void DestroyBox(BoxData fBoxData)
    {
        var pos3D = BoxHelper.Pos2DTo3DBox(fBoxData.Pos2D);
        boxPool.MyDestroy(boxModel3DDic[pos3D]);
        boxModel3DDic.Remove(pos3D);
    }
    #endregion
    
    
    public static BoxModel FirstBoxModel => boxModel3DDic.First();
}