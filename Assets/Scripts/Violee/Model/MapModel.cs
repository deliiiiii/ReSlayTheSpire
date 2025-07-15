using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Violee
{
    public class MapModel : MonoBehaviour
    {
        async Task LoadConfig()
        {
            if (BoxConfig == null)
                return;
            BoxConfig.BoxConfigs = new List<BoxConfigSingle>();
            var textures = await Resourcer.LoadAssetsAsyncByLabel<Texture2D>("BoxFigma");
            textures.ForEach(t =>
            {
                var match = Regex.Match(t.name, @"\d+");
                var id = match.Success ? byte.Parse(match.Value) : new byte();
                var boxConfig = new BoxConfigSingle()
                {
                    Name = t.name,
                    Walls = id,
                    Texture2D = t,
                    // 强制刷新所有权重
                    BasicWeight = 100,
                };
                BoxConfigs.Add(boxConfig);
            });
#if UNITY_EDITOR
            EditorUtility.SetDirty(BoxConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            BoxConfigs.Sort((x, y) => x.Walls - y.Walls);
            Debug.Log("LoadConfig Completed");
        }
        async void Awake()
        {
            if(RefreshConfigOnAwake)
            {
                await LoadConfig();
            }

            
            emptyBoxConfig = BoxConfigs.First(x => x.Walls == 0);
            BoxConfigs.ForEach(boxConfig =>
            {
                var t = boxConfig.Texture2D;
                boxConfig.Sprite = Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f,
                    0,
                    SpriteMeshType.Tight);
            });
            
            allBoxWalls = BoxConfigs.Select(x => x.Walls).Distinct().ToList();
            allBoxSides = (EBoxDir[])Enum.GetValues(typeof(EBoxDir));
            
            canGoOutDirsDic = new Dictionary<byte, List<EBoxDir>>();
            allBoxWalls.ForEach(w =>
            {
                canGoOutDirsDic.Add(w, new List<EBoxDir>());
                allBoxSides.ForEach(dir =>
                {
                    if (BoxData.CanGoOutAt(w, dir))
                    {
                        canGoOutDirsDic[w].Add(dir);
                    }
                });
            });

            oppositeDirDic = new Dictionary<EBoxDir, EBoxDir>()
            {
                { EBoxDir.Up, EBoxDir.Down },
                { EBoxDir.Down, EBoxDir.Up },
                { EBoxDir.Left, EBoxDir.Right },
                { EBoxDir.Right, EBoxDir.Left }
            };
            
            PlayerModel.OnInputMove += OnPlayerInputMove;
        }
        public static event Action OnGenerateMap;
        public static event Action<Vector2Int, BoxData> OnAddBox;
        public static event Action<Vector2Int> OnRemoveBox;
        public static event Action<Vector2Int> OnInputEnd;

        public bool RefreshConfigOnAwake;
        public int YieldCount;

        
        #region BoxConfig
        [SerializeField]
        BoxConfig BoxConfig;
        List<BoxConfigSingle> BoxConfigs => BoxConfig.BoxConfigs;
        BoxConfigSingle emptyBoxConfig;
        List<byte> allBoxWalls;
        EBoxDir[] allBoxSides;
        /// <summary>
        /// (walls, [outDir1, ...])
        /// </summary>
        Dictionary<byte, List<EBoxDir>> canGoOutDirsDic;
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        Dictionary<EBoxDir, EBoxDir> oppositeDirDic;
        readonly Dictionary<EBoxDir, Vector2Int> dirToVec2Dic = new()
        {
            { EBoxDir.Up, new Vector2Int(0, 1) },
            { EBoxDir.Down, new Vector2Int(0, -1) },
            { EBoxDir.Left, new Vector2Int(-1, 0) },
            { EBoxDir.Right, new Vector2Int(1, 0) }
        };
        Vector2Int NextPos(Vector2Int thisLoc, EBoxDir dir)
        {
            return new Vector2Int(thisLoc.x + dirToVec2Dic[dir].x, thisLoc.y + dirToVec2Dic[dir].y);
        }
        #endregion
        
        
        List<(Vector2Int, EBoxDir)> GetNextLocAndDirList(Vector2Int thisPos)
        {
            var nextPoses = new List<(Vector2Int, EBoxDir)>();
            allBoxSides.ForEach(dir =>
            {
                nextPoses.Add((NextPos(thisPos, dir), oppositeDirDic[dir]));
            });
            return nextPoses;
        }
        
        
        #region Map
        public int Height = 4;
        public int Width = 6;
        public Vector2Int StartPos;
        EBoxDir startDir = EBoxDir.Up;
        MapData mapData;
        List<Vector2Int> emptyPosList;
        
        bool InMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
        bool HasBox(Vector2Int pos) => mapData.Contains(pos);
        async Task<BoxData> AddBox(Vector2Int pos, BoxConfigSingle config)
        {
            await YieldFrames();
            var t = config.Texture2D;
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

        async Task GenerateOneFakeConnection(bool startWithStartLoc)
        {
            var edgeBoxStack = new Stack<BoxData>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = startWithStartLoc ? StartPos : emptyPosList[0];
            var firstBox =  await AddBox(firstLoc, emptyBoxConfig);
            edgeBoxStack.Push(firstBox);
            while (edgeBoxStack.Count > 0)
            {
                var curBox = edgeBoxStack.Pop();
                var curWall = curBox.Walls;
                var nextPairs = GetNextLocAndDirList(curBox.Pos);
                foreach (var pair in nextPairs)
                {
                    if (InMap(pair.Item1) 
                        && !HasBox(pair.Item1) 
                        && canGoOutDirsDic[curWall].Contains(oppositeDirDic[pair.Item2]))
                    {
                        var wall = 
                            BoxConfigs.RandomItemWeighted(
                                x => canGoOutDirsDic[x.Walls].Contains(pair.Item2),
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
            mapData.ForEach(boxValue => boxValue.InitPoint(allBoxSides));
            var startBox = mapData[StartPos].BoxPointDic[startDir];
            startBox.CostWall.Value = 0;
            pq.Enqueue(startBox, 0);
            while (pq.Count != 0)
            {
                var curBoxPoint = pq.Dequeue();
                curBoxPoint.UpdateCostInBox();
                // NextPos()
            }
        }
        #endregion
        
        
        #region Event
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