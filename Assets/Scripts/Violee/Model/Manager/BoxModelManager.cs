using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;

internal class BoxModelManager : ModelManagerBase<BoxModel, BoxModelManager>
{
    #region Inspector
    [Header("Map Settings")]
    [SerializeField] int height = 4;
    [SerializeField] int width = 6;
    [SerializeField] Vector2Int startPos;
    [SerializeField] EBoxDir startDir = EBoxDir.Up;
    #endregion
    
    
    #region Public Event & Functions
    public static event Action? OnBeginGenerate;
    public static event Action? OnEndGenerate;
    public static event Func<Task>? OnBeginDij;
    public static event Action<Vector3>? OnEndDij;
    public static List<BoxPointData> GetAllPoints()
    {
        return boxKList.SelectMany(x => x.PointKList).ToList();
    }
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
            if (Math.Abs(x - edgeX) + Math.Abs(z - edgeZ) <= BoxHelper.BoxSize * Configer.BoxConfig.WalkInTolerance
                && !pointData.Visited)
            {
                pointData.Visited.Value = true;
                MyDebug.Log($"Enter Point!!{boxPos2D}:{dir}");
            }
        }
    }

    public static readonly GuardedFunc<Task> StartGenerateFunc = new (_StartGenerate);
    public static readonly GuardedFunc<Task> DijkstraFunc = new (_Dijkstra);
    public static float MaxSize => Mathf.Max(Width, Height) * BoxHelper.BoxSize;
    #endregion
    
    
    #region PosInMap, Box
    static int Height => Instance.height;
    static int Width => Instance.width;
    static Vector2Int StartPos => Instance.startPos;
    static EBoxDir StartDir => Instance.startDir;
    static readonly MyKeyedCollection<Vector2Int, BoxData> boxKList = new (b => b.Pos2D);
    static bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    static bool HasBox(Vector2Int pos) => boxKList.Contains(pos);
    static async Task<BoxData> AddBoxAsync(Vector2Int pos, BoxConfigSingle config)
    {
        await Configer.SettingsConfig.YieldFrames();
        var boxData = new BoxData(pos, config);
        MyDebug.Log($"Add box {config.Walls} at {pos}");
        boxKList.Add(boxData);
        emptyPosSet.Remove(pos);
        await SpawnBox3D(boxData);
        return boxData;
    }
    static void RemoveBox(BoxData boxData)
    {
        boxKList.Remove(boxData);
        emptyPosSet.Add(boxData.Pos2D);
        DestroyBox(boxData);
    }
    static void RemoveAllBoxes()
    {
        boxKList.ForEach(DestroyBox);
        boxKList.Clear();
        emptyPosSet.Clear();
        for(int j = 0; j < Height; j++)
        {
            for(int i = 0; i < Width; i++)
            {
                emptyPosSet.Add(new Vector2Int(i, j));
            }
        }
    }
    #endregion


    #region Generate
    static readonly HashSet<Vector2Int> emptyPosSet = [];
    static async Task _StartGenerate()
    {
        try
        {
            OnBeginGenerate?.Invoke();
            RemoveAllBoxes();
            await GenerateOneFakeConnection(true);
            while (emptyPosSet.Count > 0)
            {
                await GenerateOneFakeConnection(false);
            }
            OnEndGenerate?.Invoke();
            await (DijkstraFunc.TryInvoke() ?? Task.CompletedTask);
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    static async Task GenerateOneFakeConnection(bool startWithStartLoc)
    {
        try
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : emptyPosSet.First();
            var firstBox = await AddBoxAsync(firstLoc, BoxHelper.EmptyBoxConfig);
            edgeBoxStack.Push(firstBox);
            while (edgeBoxStack.Count > 0)
            {
                var curBox = edgeBoxStack.Pop();
                var nextPairs = BoxHelper.GetNextLocAndGoInDirList(curBox.Pos2D);
                foreach (var nextPair in nextPairs)
                {
                    // “下一格”
                    var nextPos = nextPair.Item1;
                    var nextGoInDir = nextPair.Item2;
                    var curGoOutDir = BoxHelper.OppositeDirDic[nextPair.Item2];
                    if (!InMap(nextPos))
                    {
                        curBox.AddSWall(new WallData(curGoOutDir, EDoorType.None));
                        // MyDebug.Log($"ReachMapEdge, AddWall {curBox.Pos}:{curGoOutDir}");
                        continue;
                    }
                    if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir, out _))
                    {
                        var boxConfig = 
                            Configer.BoxConfig.BoxConfigList.RandomItemWeighted(
                                x => !BoxData.HasSWallByByteAndDir(x.Walls, nextGoInDir),
                                x => x.BasicWeight);
                        var nextBox = await AddBoxAsync(nextPos, boxConfig);
                        var nextNextPairs = BoxHelper.GetNextLocAndGoInDirList(nextPos);
                        foreach (var nextNextPair in nextNextPairs)
                        {
                            var nextNextPos = nextNextPair.Item1;
                            // “下一格”的相邻格的走入方向
                            var nextNextGoInDir = nextNextPair.Item2;
                            var nextGoOutDir = BoxHelper.OppositeDirDic[nextNextPair.Item2];
                            if (InMap(nextNextPos) && HasBox(nextNextPos))
                            {
                                var nextNextBox = boxKList[nextNextPos];
                                if (nextNextBox.HasSWallByDir(nextNextGoInDir, out _))
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
    static async Task _Dijkstra()
    {
        try
        {
            MyDebug.Log("Dijkstra 1");
            foreach (var boxData in boxKList)
            {
                boxData.ResetCost();
            }
            if (OnBeginDij != null)
            {
                await OnBeginDij.Invoke();
            }
            MyDebug.Log("Dijkstra 2");
            var vSet = new HashSet<BoxPointData>();
        
            var pq = new SimplePriorityQueue<BoxPointData, int>();
            var startBox = boxKList[StartPos];
            var startPoint = startBox.PointKList[StartDir];
            startPoint.CostWall.Value = 0;
            pq.Enqueue(startPoint, 0);
            while (pq.Count != 0)
            {
                var curPoint = pq.Dequeue();
                vSet.Add(curPoint);
                var curCost = curPoint.CostWall;
                var curBox = curPoint.BelongBox;
                curPoint.UpdateNextPointCost();
                var nextPos = BoxHelper.NextPos(curBox.Pos2D, curPoint.Dir);
                if (InMap(nextPos))
                {
                    var nextBox = boxKList[nextPos];
                    var oppositeDir = BoxHelper.OppositeDirDic[curPoint.Dir];
                    var nextPoint = nextBox.PointKList[oppositeDir];
                    nextPoint.CostWall.Value = Math.Min(
                        nextPoint.CostWall.Value,
                        curCost.Value + curBox.CostStraight(curPoint.Dir) + nextBox.CostStraight(oppositeDir));
                    if (!vSet.Contains(nextPoint))
                    {
                        if(pq.Contains(nextPoint))
                            pq.UpdatePriority(nextPoint, nextPoint.CostWall);
                        else
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                    }
                }
                curPoint.NextPointsInBox
                    .Where(nextPoint => !vSet.Contains(nextPoint))
                    .ForEach(nextPoint =>
                    {
                        if (pq.Contains(nextPoint))
                            pq.UpdatePriority(nextPoint, nextPoint.CostWall);
                        else
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                    });
                await Configer.SettingsConfig.YieldFrames();
            }
            MyDebug.Log("Dijkstra finished!");
            OnEndDij?.Invoke(BoxHelper.Pos2DTo3DPoint(StartPos, StartDir));
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
        var boxModel = await Instance.modelPool.MyInstantiate();
        boxModel.ReadData(fBoxData);
        boxModel3DDic.Add(boxModel);
    }
        
    static void DestroyBox(BoxData fBoxData)
    {
        var pos3D = BoxHelper.Pos2DTo3DBox(fBoxData.Pos2D);
        Instance.modelPool.MyDestroy(boxModel3DDic[pos3D]);
        boxModel3DDic.Remove(pos3D);
    }
    #endregion
}