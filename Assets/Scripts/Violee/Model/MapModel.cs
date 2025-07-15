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
        void Awake()
        {
            PlayerModel.OnInputMove += OnPlayerInputMove;
        }
        public int YieldCount;
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
            var boxData = new BoxData()
            {
                Pos = pos,
                Walls = config.Walls,
                Sprite = config.Sprite,
            };
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
            emptyPosList = new List<Vector2Int>();
            for(int j = 0; j < Height; j++)
            {
                for(int i = 0; i < Width; i++)
                {
                    emptyPosList.Add(new Vector2Int(i, j));
                }
            }
        }
        #endregion

        async Task GenerateOneFakeConnection(bool startWithStartLoc)
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : emptyPosList[0];
            var firstBox =  await AddBox(firstLoc, BoxHelper.emptyBoxConfig);
            edgeBoxStack.Push(firstBox);
            while (edgeBoxStack.Count > 0)
            {
                var curBox = edgeBoxStack.Pop();
                var curWall = curBox.Walls;
                var nextPairs = BoxHelper.GetNextLocAndDirList(curBox.Pos);
                foreach (var pair in nextPairs)
                {
                    if (InMap(pair.Item1) 
                        && !HasBox(pair.Item1) 
                        && BoxHelper.canGoOutDirsDic[curWall].Contains(BoxHelper.oppositeDirDic[pair.Item2]))
                    {
                        var wall = 
                            BoxConfigList.RandomItemWeighted(
                                x => BoxHelper.canGoOutDirsDic[x.Walls].Contains(pair.Item2),
                                x => x.BasicWeight);
                        var nextBox = await AddBox(pair.Item1, wall);
                        edgeBoxStack.Push(nextBox);
                    }
                }
            }
        }

        async Task YieldFrames()
        {
            for (int y = 0; y < YieldCount; y++)
            {
                await Task.Yield();
            }
        }

        // 防止点击多次按钮
        bool isGenerating;
        [Button]
        async Task StartGenerate()
        {
            if (isGenerating)
                return;
            isGenerating = true;
            RemoveAllBoxes();
            mapData = new MapData();
            await GenerateOneFakeConnection(true);
            while (emptyPosList.Count > 0)
            {
                await GenerateOneFakeConnection(false);
            }

            // await Dijkstra();
            OnGenerateMap?.Invoke();
            isGenerating = false;
        }

        SimplePriorityQueue<BoxPointData, int> pq;
        async Task Dijkstra()
        {
            pq = new SimplePriorityQueue<BoxPointData, int>();
            var startBox = mapData[StartPos];
            var startPoint = startBox.PointDic[StartDir];
            startPoint.CostWall.Value = 0;
            pq.Enqueue(startPoint, 0);
            while (pq.Count != 0)
            {
                var curPoint = pq.Dequeue();
                var curCost = curPoint.CostWall;
                var curBox = curPoint.BelongBox;
                curPoint.UpdateNextPointCost();
                var nextPos = BoxHelper.NextPos(curBox.Pos, curPoint.Dir);
                var nextBox = mapData[nextPos];
                if (InMap(nextPos))
                {
                    var oppositeDir = BoxHelper.oppositeDirDic[curPoint.Dir];
                    var nextPoint = nextBox.PointDic[oppositeDir];
                    var nextCostOb = nextPoint.CostWall;
                    nextCostOb.Value = Math.Min(
                        nextCostOb.Value,
                        curCost.Value + curBox.CostStraight(curPoint.Dir) + nextBox.CostStraight(oppositeDir));
                    pq.Enqueue(nextPoint, nextCostOb.Value);
                }
                foreach (var n in curPoint.NextPointsInBox)
                {
                    pq.Enqueue(n, n.CostWall);
                }
                await YieldFrames();
            }
        }


        #region Function Got By Editor
        public static List<BoxPointData> GetAllPoints()
        {
            return mapData?.SelectMany(x => x.PointDic.Values).ToList() ?? new List<BoxPointData>();
        }
        #endregion
        
        #region Event
        public static event Action OnGenerateMap;
        public static event Action<Vector2Int, BoxData> OnAddBox;
        public static event Action<Vector2Int> OnRemoveBox;
        public static event Action<Vector2Int> OnInputEnd;
        void OnPlayerInputMove(int curX, int curY, int dx, int dy)
        {
            var nextLoc = new Vector2Int(curX + dx, curY + dy);
            if (!InMap(nextLoc))
                return;
            OnInputEnd?.Invoke(nextLoc);
        }
        #endregion
    }
}