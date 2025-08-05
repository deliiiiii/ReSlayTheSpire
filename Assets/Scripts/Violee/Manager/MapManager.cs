using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Violee;

public struct DijStreamParam(
    SerializableDictionary<Vector2Int, BoxData> b, 
    Vector3 v)
{
    public readonly SerializableDictionary<Vector2Int, BoxData> BoxDataDic = b;
    public readonly Vector3 PlayerStartPos = v;
}



internal class MapManager : SingletonCS<MapManager>
{
    static readonly BoxConfigList boxConfigList;
    static readonly ObjectPool<BoxModel> boxPool;
    static MapManager()
    {
        boxConfigList = Configer.BoxConfigList;
        boxPool = new ObjectPool<BoxModel>(Configer.BoxModel, Instance.go.transform, 42);
        
        GenerateStream = Instance.ToStream(StartGenerate);
        DijkstraStream = Instance
            .Bind(() => new DijStreamParam(boxDataDic, BoxHelper.Pos2DTo3DPoint(StartPos, StartDir)))
            .ToStreamAsync(Dijkstra)
            .OnEnd(_ => VisitEdgeWalls());
        GenerateStream.EndWith(DijkstraStream);
    }
    public static float MaxSize => Mathf.Max(Width, Height) * BoxHelper.BoxSize;
    static int Height => boxConfigList.Height;
    static int Width => boxConfigList.Width;
    static Vector2Int StartPos => boxConfigList.StartPos;
    static EBoxDir StartDir => boxConfigList.StartDir;
    static bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    static bool HasBox(Vector2Int pos) => boxDataDic.ContainsKey(pos);
    
    
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
            var pointData = boxDataDic[boxPos2D].PointDataMyDic[dir];
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

    static void VisitEdgeWalls()
    {
        edgeWallSet.ForEach(wallData => wallData.Visited.Value = true);
    }
    #endregion

    #region DrawSceneItems

    public static void DrawAtWall(WallData wallData, DrawConfig config)
    {
        var points = playerCurPoint.Value.AtWallGetInsidePoints(wallData)
            .Where(p => !p.BelongBox.OccupiedDirs.Contains(p.Dir)).ToList();
        config.ToDrawModels.ForEach(model =>
        {
            var p = points.RandomItem();
            if (p == null)
            {
                MyDebug.Log("No Enough Points...");
                return;
            }
            points.Remove(p);
            p.BelongBox.SceneDataMyList.MyAdd(model.Data.CreateNew([p.Dir]));
        });
    }

    #endregion
    
    
    #region Generate

    static HashSet<Vector2Int> emptyPosSet = [];
    static HashSet<WallData> edgeWallSet = [];
    public static readonly Stream<ValueTuple> GenerateStream;
    public static readonly Stream<DijStreamParam> DijkstraStream;
    static void StartGenerate()
    {
        boxDataDic.Clear();
        emptyPosSet =
        [..
            Enumerable.Range(0, Width)
                .SelectMany(i => Enumerable.Range(0, Height)
                    .Select(j => new Vector2Int(i, j)))
        ];
        edgeWallSet = [];
        GenerateOneFakeConnection(true);
        while (emptyPosSet.Count > 0)
        {
            GenerateOneFakeConnection(false);
        }
    }
    static void GenerateOneFakeConnection(bool startWithStartLoc)
    {
        static BoxData ReadBoxConfig(Vector2Int pos, BoxConfig config) 
            => new(pos, config);
        var edgeBoxStack = new Stack<BoxData>();
        // 起始位置是空格子
        var firstLoc = startWithStartLoc ? StartPos : emptyPosSet.First();
        var firstConfig = startWithStartLoc 
            ? BoxHelper.EmptyBoxConfig
            : Configer.BoxConfigList.BoxConfigs.RandomItem(weightFunc: x => x.BasicWeight);
        var firstBox = ReadBoxConfig(firstLoc, firstConfig);
        boxDataDic.Add(firstLoc, firstBox);
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
                    var edgeWall = new WallData(curGoOutDir, EDoorType.None){BelongBox = curBox};
                    edgeWallSet.Add(edgeWall);
                    curBox.WallDataMyDic.Remove(edgeWall.WallType);
                    curBox.WallDataMyDic.Add(edgeWall.WallType, edgeWall);
                    // MyDebug.Log($"ReachMapEdge, AddWall {curBox.Pos}:{curGoOutDir}");
                    continue;
                }
                var boxConfig = 
                    Configer.BoxConfigList.BoxConfigs.RandomItem(
                        x => !BoxHelper.HasSWallByByteAndDir(x.Walls, nextGoInDir),
                        x => x.BasicWeight);
                var nextBox = ReadBoxConfig(nextPos, boxConfig);
                if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir, out _))
                {
                    boxDataDic.Add(nextPos, nextBox);
                    edgeBoxStack.Push(nextBox);
                }
            }
        }
    }
    static async Task Dijkstra(DijStreamParam param)
    {
        try
        {
            boxDataDic.Values.ForEach(boxData => boxData.ResetBeforeDij());
            var vSet = new HashSet<BoxPointData>();
            var pq = new SimplePriorityQueue<BoxPointData, int>();
            var startBox = boxDataDic[StartPos];
            var startPoint = startBox.PointDataMyDic[StartDir];
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
                    var nextBox = boxDataDic[nextPos];
                    var oppositeDir = BoxHelper.OppositeDirDic[curDir];
                    var nextPoint = nextBox.PointDataMyDic[oppositeDir];
                    var costSWall = curBox.CostStraight(curDir, out var wallData1) + nextBox.CostStraight(oppositeDir, out var wallData2);
                    nextPoint.CostWall.Value = Math.Min(
                        nextPoint.CostWall.Value,
                        curCost.Value + costSWall);
                    if (wallData1 != null)
                        curPoint.AddWallAndNextPoint((nextPoint, wallData1));
                    if (wallData2 != null)
                        curPoint.AddWallAndNextPoint((nextPoint, wallData2));
                    
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
                        var costTWall = curBox.CostTilt(curPoint.Dir, nextPoint.Dir, out var wallData3);
                        if(wallData3 != null)
                            curPoint.AddWallAndNextPoint((nextPoint, wallData3));
                        if (!vSet.Contains(nextPoint))
                        {
                            nextPoint.CostWall.Value = Math.Min(
                                nextPoint.CostWall.Value,
                                curPoint.CostWall + costTWall);
                            if (pq.Contains(nextPoint))
                                pq.UpdatePriority(nextPoint, nextPoint.CostWall);
                            else
                                pq.Enqueue(nextPoint, nextPoint.CostWall);
                            if(costTWall == 0)
                                curPoint.Merge(nextPoint);
                        }
                    });
                await Configer.SettingsConfig.YieldFrames();
            }
            MyDebug.Log("Dijkstra finished!");
            if (!CheckConnective())
                await Dijkstra(param);
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }

    static bool CheckConnective()
    {
        var invalidWallData = boxDataDic.Values
            .SelectMany(b => b.PointDataMyDic.Values.SelectMany(p => p.InvalidWalls()))
            .ToList()
            .RandomItem();
        if (invalidWallData == null)
            return true;
        var boxData = invalidWallData.BelongBox;
        boxData.WallDataMyDic.Remove(invalidWallData.WallType);
        boxData.WallDataMyDic.Add(invalidWallData.WallType, new WallData(invalidWallData.WallType, EDoorType.Wooden){BelongBox = boxData});
        MyDebug.Log($"ReplaceWall At {boxData.Pos2D}.{invalidWallData.WallType}");
        return false;
    }
    
    static readonly SerializableDictionary<Vector2Int, BoxData> boxDataDic 
        = new (
            boxData =>
            {
                Task.FromResult(OnAddBoxData(boxData));
                emptyPosSet.Remove(boxData.Pos2D);
                
                var nextPairs = BoxHelper.GetNextLocAndGoInDirList(boxData.Pos2D);
                foreach (var (nextPos, nextGoInDir) in nextPairs)
                {
                    // nextGoInDir: 相邻格的走入方向
                    var goOutDir = BoxHelper.OppositeDirDic[nextGoInDir];
                    if (InMap(nextPos) && HasBox(nextPos))
                    {
                        var nextBox = boxDataDic![nextPos];
                        if (nextBox.HasSWallByDir(nextGoInDir, out _))
                        {
                            var t = BoxHelper.WallDirToType(goOutDir);
                            boxData.WallDataMyDic.Remove(t);
                            // MyDebug.Log($"WallRepeat, RemoveWall {nextBox.Pos}:{nextGoOutDir}");
                        }
                    }
                }
            },
            boxData =>
            {
                OnRemoveBoxData(boxData);
                emptyPosSet.Add(boxData.Pos2D);
            });

    [JsonIgnore] static readonly SerializableDictionary<Vector3Int, BoxModel> boxModelDic = [];

    static async Task OnAddBoxData(BoxData boxData)
    {
        await Configer.SettingsConfig.YieldFrames();
        var boxModel = await boxPool.MyInstantiate();
        boxModel.ReadData(boxData);
        var pos3D = BoxHelper.Pos2DTo3DBox(boxData.Pos2D);
        boxModelDic.Add(pos3D, boxModel);
    }
    static void OnRemoveBoxData(BoxData boxData)
    {
        var pos3D = BoxHelper.Pos2DTo3DBox(boxData.Pos2D);
        boxPool.MyDestroy(boxModelDic[pos3D]);
        boxModelDic.Remove(pos3D);
    }
    #endregion
    
    
    public static BoxData BoxDataByPos(Vector2Int pos) => boxDataDic[pos];

    public static void AddTest(BoxData boxData)
    {
        boxDataDic.Remove(boxData.Pos2D);
        boxDataDic.Add(boxData.Pos2D, boxData);
    }
}