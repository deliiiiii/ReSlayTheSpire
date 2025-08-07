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


public class GenerateParam
{
    readonly MapData mapData;
    readonly ObjectPool<BoxModel> boxPool;
    readonly BoxConfigList boxConfigList;
    public int Height => boxConfigList.Height;
    public int Width => boxConfigList.Width;
    public Vector2Int StartPos => boxConfigList.StartPos;
    public EBoxDir StartDir => boxConfigList.StartDir;
    public MyDictionary<Vector2Int, BoxData> BoxDataDic => mapData.BoxDataDic;
    public DateTime DateTime
    {
        get => mapData.DateTime;
        set => mapData.DateTime = value;
    }

    public bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    public bool HasBox(Vector2Int pos) => BoxDataDic.ContainsKey(pos);

    
    public HashSet<Vector2Int> EmptyPosSet = [];
    public HashSet<WallData> EdgeWallSet = [];
    public readonly Vector3 PlayerStartPos;
    readonly Dictionary<Vector3Int, BoxModel> boxModelDic = [];

    async Task OnAddBoxData(BoxData boxData)
    {
        await Configer.SettingsConfig.YieldFrames();
        var boxModel = await boxPool.MyInstantiate();
        boxModel.ReadData(boxData);
        var pos3D = BoxHelper.Pos2DTo3DBox(boxData.Pos2D);
        boxModelDic.Add(pos3D, boxModel);
    }
    void OnRemoveBoxData(BoxData boxData)
    {
        var pos3D = BoxHelper.Pos2DTo3DBox(boxData.Pos2D);
        boxPool.MyDestroy(boxModelDic[pos3D]);
        boxModelDic.Remove(pos3D);
    }
    public GenerateParam(MapData mapData, GameObject go)
    {
        this.mapData = mapData;
        
        boxConfigList = Configer.BoxConfigList;
        boxPool = new ObjectPool<BoxModel>(Configer.BoxModel, go.transform, 42);
        
        BoxDataDic.OnAdd += boxData =>
            {
                Task.FromResult(OnAddBoxData(boxData));
                EmptyPosSet.Remove(boxData.Pos2D);

                var nextPairs = BoxHelper.GetNextLocAndGoInDirList(boxData.Pos2D);
                foreach (var (nextPos, nextGoInDir) in nextPairs)
                {
                    // nextGoInDir: 相邻格的走入方向
                    var goOutDir = BoxHelper.OppositeDirDic[nextGoInDir];
                    if (InMap(nextPos) && HasBox(nextPos))
                    {
                        var nextBox = BoxDataDic[nextPos];
                        if (nextBox.HasSWallByDir(nextGoInDir, out _))
                        {
                            var t = BoxHelper.WallDirToType(goOutDir);
                            boxData.WallDataMyDic.Remove(t);
                            // MyDebug.Log($"WallRepeat, RemoveWall {nextBox.Pos}:{nextGoOutDir}");
                        }
                    }
                }
            };
        BoxDataDic.OnRemove += boxData =>
            {
                OnRemoveBoxData(boxData);
                EmptyPosSet.Add(boxData.Pos2D);
            };
        PlayerStartPos = BoxHelper.Pos2DTo3DPoint(StartPos, StartDir);
    }
}


internal class MapManager : SingletonCS<MapManager>
{
    public static readonly Stream<ValueTuple, GenerateParam> GenerateStream;
    public static readonly Stream<GenerateParam, GenerateParam> DijkstraStream;
    public static readonly Stream<(GenerateParam, Vector3), Observable<BoxPointData>> PlayerCurPointStream;
    static readonly GenerateParam generateParam = new (new MapData(), Instance.go);
    static MapManager()
    {
        GenerateStream = Streamer
            .Bind(ValueTuple.Create)
            .SetTrigger(_ =>
            {
                var param = generateParam;
                InitCollections(param);
                GenerateMain(param);
                return param;
            });
        DijkstraStream = GenerateStream
            .ContinueAsync(Dijkstra)
            .OnEnd(param =>
            {
                VisitEdgeWalls(param.EdgeWallSet);
                param.DateTime = new DateTime(2025, 8, 14, 8, 0, 0);
            });
        PlayerCurPointStream = DijkstraStream
            .BindResult(p => (p, PlayerManager.GetPos()))
            .SetTrigger(TickPlayerVisit);
    }
    
    static readonly Observable<BoxPointData> playerCurPoint = new (null!);
    #region Visit
    public static Observable<BoxPointData> TickPlayerVisit((GenerateParam, Vector3) pair)
    {
        
        var boxDataDic = pair.Item1.BoxDataDic;
        var playerPos = pair.Item2;
        var x = playerPos.x;
        var z = playerPos.z;
        var boxPos2D = BoxHelper.Pos3DTo2D(playerPos);
        var boxPos3D = BoxHelper.Pos2DTo3DBox(boxPos2D);
        if (!pair.Item1.HasBox(boxPos2D))
        {
            MyDebug.LogWarning($"Why !HasBox({boxPos3D}) PlayerPos:{playerPos}");
            return playerCurPoint;
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
        return playerCurPoint;
    }

    static void VisitEdgeWalls(HashSet<WallData> edgeWallSet)
    {
        edgeWallSet.ForEach(wallData => wallData.Visited.Value = true);
    }
    #endregion
    
    
    #region SceneItems
    public static DateTime GetCurTime() => DijkstraStream.SelectResult(x => x.DateTime);

    public static void DrawAtWall(List<BoxPointData> insidePoints, WallData wallData, DrawConfig config)
    {
        MyDebug.Log("Start Draw");
        config.ToDrawModels.ForEach(model =>
        {
            var p = insidePoints.RandomItem(p => model.Data.IsAir 
                ? !p.BelongBox.OccupiedAirs.Contains(p.Dir) && p.HasSWall()
                : !p.BelongBox.OccupiedFloors.Contains(p.Dir));
            if (p == null)
            {
                MyDebug.LogWarning("No Enough Points...");
                return;
            }
            insidePoints.Remove(p);
            p.BelongBox.SceneDataMyList.MyAdd(model.Data.CreateNew([p.Dir]));
        });
    }
    #endregion
    
    
    #region Generate
    static void InitCollections(GenerateParam param)
    {
        param.BoxDataDic.Clear();
        param.EmptyPosSet =
        [..
            Enumerable.Range(0, param.Width)
                .SelectMany(i => Enumerable.Range(0, param.Height)
                    .Select(j => new Vector2Int(i, j)))
        ];
        param.EdgeWallSet = [];
    }
    static void GenerateMain(GenerateParam param)
    {
        GenerateOneFakeConnection(true, param);
        while (param.EmptyPosSet.Count > 0)
        {
            GenerateOneFakeConnection(false, param);
        }
    }
    static void GenerateOneFakeConnection(bool startWithStartLoc, GenerateParam param)
    {
        static BoxData ReadBoxConfig(Vector2Int pos, BoxConfig config) 
            => new(pos, config);
        var edgeBoxStack = new Stack<BoxData>();
        // 起始位置是空格子
        var firstLoc = startWithStartLoc ? param.StartPos : param.EmptyPosSet.First();
        var firstConfig = startWithStartLoc 
            ? Configer.BoxConfigList.BoxConfigs.First(x => x.Walls == 0)
            : Configer.BoxConfigList.BoxConfigs.RandomItem(weightFunc: x => x.BasicWeight);
        var firstBox = ReadBoxConfig(firstLoc, firstConfig);
        param.BoxDataDic.Add(firstLoc, firstBox);
        edgeBoxStack.Push(firstBox);
        while (edgeBoxStack.Count > 0)
        {
            var curBox = edgeBoxStack.Pop();
            var nextPairs = BoxHelper.GetNextLocAndGoInDirList(curBox.Pos2D);
            foreach (var (nextPos, nextGoInDir) in nextPairs)
            {
                // “下一格”
                var curGoOutDir = BoxHelper.OppositeDirDic[nextGoInDir];
                if (!param.InMap(nextPos))
                {
                    var edgeWall = new WallData(curGoOutDir, EDoorType.None){BelongBox = curBox};
                    param.EdgeWallSet.Add(edgeWall);
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
                if (!param.HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir, out _))
                {
                    param.BoxDataDic.Add(nextPos, nextBox);
                    edgeBoxStack.Push(nextBox);
                }
            }
        }
    }
    static async Task<GenerateParam> Dijkstra(GenerateParam param)
    {
        try
        {
            var boxDataDic = param.BoxDataDic;
            boxDataDic.Values.ForEach(boxData => boxData.ResetBeforeDij());
            var vSet = new HashSet<BoxPointData>();
            var pq = new SimplePriorityQueue<BoxPointData, int>();
            var startBox = boxDataDic[param.StartPos];
            var startPoint = startBox.PointDataMyDic[param.StartDir];
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
                if (param.InMap(nextPos))
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
            if (!CheckConnective(boxDataDic))
                await Dijkstra(param);
            return param;
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }

    static bool CheckConnective(MyDictionary<Vector2Int,BoxData> boxDataDic)
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
    #endregion
}