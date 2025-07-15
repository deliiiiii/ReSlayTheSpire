using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
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
            allBoxSides = (EBoxSide[])Enum.GetValues(typeof(EBoxSide));
            
            canGoOutDirsDic = new Dictionary<byte, List<EBoxSide>>();
            allBoxWalls.ForEach(w =>
            {
                canGoOutDirsDic.Add(w, new List<EBoxSide>());
                allBoxSides.ForEach(dir =>
                {
                    if (BoxData.CanGoOutAt(w, dir))
                    {
                        canGoOutDirsDic[w].Add(dir);
                    }
                });
            });

            oppositeDirDic = new Dictionary<EBoxSide, EBoxSide>()
            {
                { EBoxSide.Up, EBoxSide.Down },
                { EBoxSide.Down, EBoxSide.Up },
                { EBoxSide.Left, EBoxSide.Right },
                { EBoxSide.Right, EBoxSide.Left }
            };
            
            PlayerModel.OnInputMove += OnPlayerInputMove;
        }
        public static event Action OnGenerateMap;
        public static event Action<Loc, BoxData> OnAddBox;
        public static event Action<Loc> OnRemoveBox;
        public static event Action<Loc> OnInputEnd;

        public bool RefreshConfigOnAwake;
        public int YieldCount;

        
        #region BoxConfig
        [SerializeField]
        BoxConfig BoxConfig;
        List<BoxConfigSingle> BoxConfigs => BoxConfig.BoxConfigs;
        BoxConfigSingle emptyBoxConfig;
        List<byte> allBoxWalls;
        EBoxSide[] allBoxSides;
        /// <summary>
        /// (walls, [outDir1, ...])
        /// </summary>
        Dictionary<byte, List<EBoxSide>> canGoOutDirsDic;
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        Dictionary<EBoxSide, EBoxSide> oppositeDirDic;
        #endregion
        
        
        List<(Loc, EBoxSide)> GetNextLocAndDirList(Loc thisLoc)
        {
            var nextLocs = new List<(Loc, EBoxSide)>();
            allBoxSides.ForEach(dir =>
            {
                var (dx, dy) = dir switch
                {
                    EBoxSide.Up => (0, 1),
                    EBoxSide.Down => (0, -1),
                    EBoxSide.Left => (-1, 0),
                    EBoxSide.Right => (1, 0),
                    _ => (0, 0),
                };
                nextLocs.Add((new Loc(thisLoc.X + dx, thisLoc.Y + dy), oppositeDirDic[dir]));
            });
            return nextLocs;
        }
        
        
        #region Map
        public int Height = 4;
        public int Width = 6;
        MapData mapData;
        List<Loc> emptyLocSet;
        
        bool InMap(Loc loc) => loc.X >= 0 && loc.X < Width && loc.Y >= 0 && loc.Y < Height;
        bool HasBox(Loc loc) => mapData.BoxDic.ContainsKey(loc);
        async Task AddBox(Loc loc, BoxConfigSingle boxConfigSingle)
        {
            await YieldFrames();
            var t = boxConfigSingle.Texture2D;
            var boxData = new BoxData(boxConfigSingle.Walls, boxConfigSingle.Sprite);
            mapData.BoxDic.Add(loc, boxData);
            emptyLocSet.Remove(loc);
            OnAddBox?.Invoke(loc, boxData);
            MyDebug.Log($"Add box {boxConfigSingle.Walls} at {loc}");
        }
        void RemoveBox(Loc loc)
        {
            mapData.BoxDic.Remove(loc);
            emptyLocSet.Add(loc);
            OnRemoveBox?.Invoke(loc);
        }
        void RemoveAllBoxes()
        {
            mapData?.BoxDic?.Keys.ForEach(loc => OnRemoveBox?.Invoke(loc));
            mapData?.BoxDic?.Clear();
            emptyLocSet = new List<Loc>();
            for(int j = 0; j < Height; j++)
            {
                for(int i = 0; i < Width; i++)
                {
                    emptyLocSet.Add(new Loc(i, j));
                }
            }
        }

        async Task GenerateOneFakeConnection()
        {
            var edgeLocStack = new Stack<Loc>();
            // 每个伪连通块的第一个是空格子
            var firstLoc = emptyLocSet[0];
            await AddBox(firstLoc, emptyBoxConfig);
            edgeLocStack.Push(firstLoc);
            while (edgeLocStack.Count > 0)
            {
                var curEdgeLoc = edgeLocStack.Pop();
                var curWall = mapData.BoxDic[curEdgeLoc].Walls;
                var nextPairs = GetNextLocAndDirList(curEdgeLoc);
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
                        await AddBox(pair.Item1, wall);
                        edgeLocStack.Push(pair.Item1);
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
        [Button]
        async Task StartGenerate(Loc startLoc)
        {
            RemoveAllBoxes();
            mapData = new MapData()
            {
                BoxDic = new SerializableDictionary<Loc, BoxData>()
            };
            while (emptyLocSet.Count > 0)
            {
                await GenerateOneFakeConnection();
            }

            await AStar();
            OnGenerateMap?.Invoke();
        }

        async Task AStar()
        {
            await Task.Delay(0);
        }
        #endregion
        
        
        #region Event
        void OnPlayerInputMove(int curX, int curY, int dx, int dy)
        {
            var nextLoc = new Loc(curX + dx, curY + dy);
            if (!InMap(nextLoc))
                return;
            OnInputEnd?.Invoke(nextLoc);
        }
        #endregion
    }
}