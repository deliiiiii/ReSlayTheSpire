using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class MapModel : Singleton<MapModel>
    {
        #region Public Functions
        public static List<BoxPointData> GetAllPoints()
        {
            return boxKList?.SelectMany(x => x.PointKList).ToList() ?? new List<BoxPointData>();
        }
        #endregion
        
        
        #region PosInMap, Box
        [Header("Map Settings")]
        public int Height = 4;
        public int Width = 6;
        public Vector2Int StartPos;
        public EBoxDir StartDir = EBoxDir.Up;
        static readonly MyKeyedCollection<Vector2Int, BoxData> boxKList = new (b => b.Pos);
        bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
        bool HasBox(Vector2Int pos) => boxKList.Contains(pos);
        async Task<BoxData> AddBoxAsync(Vector2Int pos, BoxConfigSingle config)
        {
            await Configer.SettingsConfig.YieldFrames();
            var boxData = BoxData.Create(pos, config);
            MyDebug.Log($"Add box {config.Walls} at {pos}");
            boxKList.Add(boxData);
            emptyPosSet.Remove(pos);
            OnAddBoxAsync?.Invoke(boxData);
            return boxData;
        }
        void RemoveBox(BoxData boxData)
        {
            boxKList.Remove(boxData);
            emptyPosSet.Add(boxData.Pos);
            OnRemoveBox?.Invoke(boxData);
        }
        void RemoveAllBoxes()
        {
            boxKList?.ForEach(boxData => OnRemoveBox?.Invoke(boxData));
            boxKList?.Clear();
            emptyPosSet = new ();
            for(int j = 0; j < Height; j++)
            {
                for(int i = 0; i < Width; i++)
                {
                    emptyPosSet.Add(new(i, j));
                }
            }
        }
        #endregion


        #region Generate
        HashSet<Vector2Int> emptyPosSet;
        async Task GenerateOneFakeConnection(bool startWithStartLoc)
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : emptyPosSet.First();
            var firstBox = await AddBoxAsync(firstLoc, BoxHelper.EmptyBoxConfig);
            edgeBoxStack.Push(firstBox);
            while (edgeBoxStack.Count > 0)
            {
                var curBox = edgeBoxStack.Pop();
                var nextPairs = BoxHelper.GetNextLocAndGoInDirList(curBox.Pos);
                foreach (var nextPair in nextPairs)
                {
                    // “下一格”
                    var nextPos = nextPair.Item1;
                    var nextGoInDir = nextPair.Item2;
                    var curGoOutDir = BoxHelper.OppositeDirDic[nextPair.Item2];
                    if (!InMap(nextPos))
                    {
                        curBox.AddSWall(WallData.Create(curGoOutDir, EDoorType.None));
                        // MyDebug.Log($"ReachMapEdge, AddWall {curBox.Pos}:{curGoOutDir}");
                        continue;
                    }
                    if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir, out _))
                    {
                        var boxConfig = 
                            Configer.BoxConfig.BoxConfigList.RandomItemWeighted(
                                x => !BoxData.HasWallByByteAndDir(x.Walls, nextGoInDir),
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
        
        // 防止点击多次按钮
        bool isGenerating;
        [Button]
        public async Task StartGenerate()
        {
            try
            {
                if (isGenerating)
                {
                    MyDebug.LogWarning("正在生成地图，请稍后再点");
                    return;
                }
                if (isDij)
                {
                    MyDebug.LogWarning("正在Dijkstra，请稍后再点");
                    return;
                }
                isGenerating = true;
                RemoveAllBoxes();
                await GenerateOneFakeConnection(true);
                while (emptyPosSet.Count > 0)
                {
                    await GenerateOneFakeConnection(false);
                }

                isGenerating = false;
                await Dijkstra();
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }

        bool isDij;
        SimplePriorityQueue<BoxPointData, int> pq;
        [Button]
        async Task Dijkstra()
        {
            if (isGenerating)
            {
                MyDebug.LogWarning("正在生成地图，请稍后再点");
                return;
            }
            if (isDij)
            {
                MyDebug.LogWarning("正在Dijkstra，请稍后再点");
                return;
            }
            try
            {
                isDij = true;
                foreach (var boxData in boxKList)
                {
                    boxData.InitPoint();
                }
                if (OnBeginDij != null)
                {
                    await OnBeginDij.Invoke();
                }
                var vSet = new HashSet<BoxPointData>();
                int c = 0;
            
                pq = new SimplePriorityQueue<BoxPointData, int>();
                var startBox = boxKList[StartPos];
                var startPoint = startBox.PointKList[StartDir];
                startPoint.CostWall.Value = 0;
                pq.Enqueue(startPoint, 0);
                while (pq.Count != 0)
                {
                    c++;
                    var curPoint = pq.Dequeue();
                    vSet.Add(curPoint);
                    var curCost = curPoint.CostWall;
                    var curBox = curPoint.BelongBox;
                    curPoint.UpdateNextPointCost();
                    var nextPos = BoxHelper.NextPos(curBox.Pos, curPoint.Dir);
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
                                pq.Remove(nextPoint);
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                        }
                    }
                    foreach (var nextPoint in curPoint.NextPointsInBox)
                    {
                        if (!vSet.Contains(nextPoint))
                        {
                            if(pq.Contains(nextPoint))
                                pq.Remove(nextPoint);
                            pq.Enqueue(nextPoint, nextPoint.CostWall);
                        }
                    }
                    await Configer.SettingsConfig.YieldFrames();
                }
                MyDebug.Log($"Dijkstra finished! Count = {c}");
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }

            isDij = false;
        }
        #endregion
        
        
        #region Event
        public static event Func<BoxData, Task> OnAddBoxAsync;
        public static event Action<BoxData> OnRemoveBox;
        public static event Func<Task> OnBeginDij;
        #endregion
    }
}