using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class MapModel : MonoBehaviour
    {
        [MinValue(0)]
        public float YieldCount;
        public int Height = 4;
        public int Width = 6;
        public Vector2Int StartPos;
        public EBoxDir StartDir = EBoxDir.Up;
        List<Vector2Int> emptyPosList;
        List<BoxConfigSingle> BoxConfigList => Configer.Instance.BoxConfig.BoxConfigList;
        static MapData mapData;
        
        #region Pos Functions
        bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
        bool HasBox(Vector2Int pos) => mapData.Contains(pos);
        async Task<BoxData> AddBox(Vector2Int pos, BoxConfigSingle config)
        {
            await YieldFrames();
            var boxData = BoxData.Create(pos, config);
            mapData.Add(boxData);
            emptyPosList.Remove(pos);
            OnAddBox?.Invoke(pos, boxData);
            MyDebug.Log($"Add box {config.Walls} at {pos}");
            return boxData;
        }
        void RemoveBox(BoxData boxData)
        {
            mapData.Remove(boxData);
            emptyPosList.Add(boxData.Pos);
            OnRemoveBox?.Invoke(boxData.Pos);
        }
        void RemoveAllBoxes()
        {
            mapData?.ForEach(boxData => OnRemoveBox?.Invoke(boxData.Pos));
            mapData?.Clear();
            emptyPosList = new ();
            for(int j = 0; j < Height; j++)
            {
                for(int i = 0; i < Width; i++)
                {
                    emptyPosList.Add(new(i, j));
                }
            }
        }
        #endregion

        async Task GenerateOneFakeConnection(bool startWithStartLoc)
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : emptyPosList[0];
            var firstBox = await AddBox(firstLoc, BoxHelper.EmptyBoxConfig);
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
                        curBox.AddSWallByDir(curGoOutDir, WallData.NoDoor);
                        MyDebug.Log($"ReachMapEdge, AddWall {curBox.Pos}:{curGoOutDir}");
                        continue;
                    }
                    if (!HasBox(nextPos) && !curBox.HasSWallByDir(curGoOutDir))
                    {
                        var boxconfig = 
                            BoxConfigList.RandomItemWeighted(
                                x => !BoxData.HasWallByByteAndDir(x.Walls, nextGoInDir),
                                x => x.BasicWeight);
                        var nextBox = await AddBox(nextPos, boxconfig);
                        var nextNextPairs = BoxHelper.GetNextLocAndGoInDirList(nextPos);
                        foreach (var nextNextPair in nextNextPairs)
                        {
                            var nextNextPos = nextNextPair.Item1;
                            // “下一格”的相邻格的走入方向
                            var nextNextGoInDir = nextNextPair.Item2;
                            var nextGoOutDir = BoxHelper.OppositeDirDic[nextNextPair.Item2];
                            if (InMap(nextNextPos) && HasBox(nextNextPos))
                            {
                                var nextNextBox = mapData[nextNextPos];
                                if (nextNextBox.HasSWallByDir(nextNextGoInDir))
                                {
                                    nextBox.RemoveSWallByDir(nextGoOutDir);
                                    MyDebug.Log($"WallRepeat, RemoveWall {nextBox.Pos}:{nextGoOutDir}");
                                }
                            }
                        }
                        
                        edgeBoxStack.Push(nextBox);
                    }
                }
            }
        }

        int countNotYieldFrame;
        async Task YieldFrames()
        {
            if (YieldCount >= 1)
            {
                for (int y = 0; y < YieldCount; y++)
                {
                    await Task.Yield();
                }
                return;
            }

            countNotYieldFrame++;
            if (1f / countNotYieldFrame <= YieldCount)
            {
                countNotYieldFrame = 0;
                await Task.Yield();
            }

        }

        // 防止点击多次按钮
        bool isGenerating;
        [Button]
        async Task StartGenerate()
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
                mapData = new MapData();
                await GenerateOneFakeConnection(true);
                while (emptyPosList.Count > 0)
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

            isDij = true;
            foreach (var boxData in mapData)
            {
                boxData.InitPoint();
            }
            if (OnBeginDij != null)
            {
                await OnBeginDij.Invoke();
            }
            var vSet = new HashSet<BoxPointData>();
            int c = 0;
            try
            {
                pq = new SimplePriorityQueue<BoxPointData, int>();
                var startBox = mapData[StartPos];
                var startPoint = startBox.PointDic[StartDir];
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
                        var nextBox = mapData[nextPos];
                        var oppositeDir = BoxHelper.OppositeDirDic[curPoint.Dir];
                        var nextPoint = nextBox.PointDic[oppositeDir];
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
                    await YieldFrames();
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


        #region Function Got By Editor
        public static List<BoxPointData> GetAllPoints()
        {
            return mapData?.SelectMany(x => x.PointDic?.Values).ToList() ?? new List<BoxPointData>();
        }
        #endregion
        
        #region Event
        public static event Action<Vector2Int, BoxData> OnAddBox;
        public static event Action<Vector2Int> OnRemoveBox;
        public static event Func<Task> OnBeginDij;
        #endregion
    }
}