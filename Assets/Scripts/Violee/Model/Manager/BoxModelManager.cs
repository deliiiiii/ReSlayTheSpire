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
    protected override void Awake()
    {
        base.Awake();
        GenerateStream.SetTriggerAsync(_StartGenerate);
        GenerateStream.EndWith(DijkstraStream);
        DijkstraStream.SetTriggerAsync(_Dijkstra);
    }

    #region Inspector
    [Header("Map Settings")]
    [SerializeField] int height = 4;
    [SerializeField] int width = 6;
    [SerializeField] Vector2Int startPos;
    [SerializeField] EBoxDir startDir = EBoxDir.Up;
    #endregion
    
    
    #region Public Event & Functions
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
    #endregion


    public class GenerateStreamParam(MyKeyedCollection<Vector2Int, BoxData> boxKList, HashSet<Vector2Int> emptyPosSet)
    {
        public readonly MyKeyedCollection<Vector2Int, BoxData> BoxKList = boxKList;
        public readonly HashSet<Vector2Int> EmptyPosSet = emptyPosSet;
    }
    #region Generate
    public static readonly Stream<GenerateStreamParam> GenerateStream 
        = new(() => new GenerateStreamParam(boxKList, []), _StartGenerate);

    public static readonly Stream<(GenerateStreamParam, Vector3)> 
        DijkstraStream = new(() => (GenerateStream.Result, BoxHelper.Pos2DTo3DPoint(StartPos, StartDir)), _Dijkstra);
    static async Task _StartGenerate(GenerateStreamParam param)
    {
        var fBoxKList = param.BoxKList;
        var fEmptyPosSet = param.EmptyPosSet;
        void RemoveAllBoxes()
        {
            fBoxKList.ForEach(DestroyBox);
            fBoxKList.Clear();
            fEmptyPosSet.Clear();
            for(int j = 0; j < Height; j++)
            {
                for(int i = 0; i < Width; i++)
                {
                    fEmptyPosSet.Add(new Vector2Int(i, j));
                }
            }
        }
        try
        {
            RemoveAllBoxes();
            await GenerateOneFakeConnection(true, fEmptyPosSet);
            while (fEmptyPosSet.Count > 0)
            {
                await GenerateOneFakeConnection(false, fEmptyPosSet);
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    static async Task GenerateOneFakeConnection(bool startWithStartLoc, HashSet<Vector2Int> fEmptyPosSet)
    {
        async Task<BoxData> AddBoxAsync(Vector2Int pos, BoxConfigSingle config)
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
                    if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir))
                    {
                        var boxConfig = 
                            Configer.BoxConfig.BoxConfigList.RandomItemWeighted(
                                x => !BoxHelper.HasSWallByByteAndDir(x.Walls, nextGoInDir),
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
    static async Task _Dijkstra((GenerateStreamParam, Vector3) pair)
    {
        try
        {
            var fBoxKList = pair.Item1.BoxKList;
            MyDebug.Log("Dijkstra 1");
            foreach (var boxData in fBoxKList)
            {
                boxData.ResetCost();
            }
            MyDebug.Log("Dijkstra 2");
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
                curPoint.UpdateNextPointCost();
                var nextPos = BoxHelper.NextPos(curBox.Pos2D, curPoint.Dir);
                if (InMap(nextPos))
                {
                    var nextBox = fBoxKList[nextPos];
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