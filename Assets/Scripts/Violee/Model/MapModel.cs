using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

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

                var sprite = Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f,
                    0,
                    SpriteMeshType.Tight
                );
                ;
                var boxConfig = new BoxConfigSingle()
                {
                    Name = t.name,
                    Walls = id,
                    Sprite = sprite,
                    BasicWeight = 1,
                };
                BoxConfig.BoxConfigs.Add(boxConfig);
                
                if(id == 0)
                    emptyBoxConfig = boxConfig;
            });
            
            Debug.Log("LoadConfig Completed");
        }
        async void Awake()
        {
            await LoadConfig();
            allBoxWalls = BoxConfig.BoxConfigs.Select(x => x.Walls).Distinct().ToList();
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
        }

        public int Height = 4;
        public int Width = 6;
        
        // [ShowInInspector]
        MapData mapData;
        Stack<Loc> edgeLocStack;
        [SerializeField]
        BoxConfig BoxConfig;
        static BoxConfigSingle emptyBoxConfig;
        static List<byte> allBoxWalls;
        static EBoxSide[] allBoxSides;
        /// <summary>
        /// (walls, [outDir1, ...])
        /// </summary>
        static Dictionary<byte, List<EBoxSide>> canGoOutDirsDic;
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        static Dictionary<EBoxSide, EBoxSide> oppositeDirDic;
        static List<(Loc, EBoxSide)> GetNextLocAndDirList(Loc thisLoc)
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
        // bool NotInMap(Loc loc) => loc.X < 0 || loc.X >= Width || loc.Y < 0 || loc.Y >= Height;
        bool InMap(Loc loc) => loc.X >= 0 && loc.X < Width && loc.Y >= 0 && loc.Y < Height;
        bool HasBox(Loc loc) => mapData.BoxDic.ContainsKey(loc);

        #region Add & Remove
        void AddBox(Loc loc, BoxConfigSingle boxConfigSingle)
        {
            var boxData = new BoxData(boxConfigSingle.Walls, boxConfigSingle.Sprite);
            mapData.BoxDic.Add(loc, boxData);
            BoxModel.OnCreateBoxData(loc, boxData);
            MyDebug.Log($"Add box {boxConfigSingle.Walls} at {loc}");
        }

        void RemoveBox(Loc loc)
        {
            mapData.BoxDic.Remove(loc);
            BoxModel.OnDestroyBoxData(loc);
        }

        void RemoveAllBoxes()
        {
            mapData?.BoxDic?.Keys.ForEach(BoxModel.OnDestroyBoxData);
            mapData?.BoxDic?.Clear();
        }
        #endregion
        
        [Button]
        public async Task StartGenerate(Loc startLoc)
        {
            RemoveAllBoxes();
            mapData = new MapData()
            {
                BoxDic = new SerializableDictionary<Loc, BoxData>()
            };
            edgeLocStack = new Stack<Loc>();
            AddBox(startLoc, emptyBoxConfig);
            edgeLocStack.Push(startLoc);
            while (edgeLocStack.Count > 0)
            {
                await Task.Yield();
                var curEdgeLoc = edgeLocStack.Pop();
                var curWall = mapData.BoxDic[curEdgeLoc].Walls;

                // GetNextLocAndDirList(curEdgeLoc)
                //     .Where(pair =>
                //         InMap(pair.Item1)
                //         && !HasBox(pair.Item1)
                //         // 当前格的当前方向可以出去
                //         && canGoOutDirsDic[curWall].Contains(oppositeDirDic[pair.Item2]))
                //     .ForEach(pair =>
                //     {
                //         var wall = allBoxWalls.RandomItem(w => canGoOutDirsDic[w].Contains(pair.Item2));
                //         AddBox(pair.Item1, wall);
                //         edgeLocStack.Push(pair.Item1);
                //     });
                
                
                var nextPairs = GetNextLocAndDirList(curEdgeLoc);
                foreach (var pair in nextPairs)
                {
                    if (InMap(pair.Item1) 
                        && !HasBox(pair.Item1) 
                        && canGoOutDirsDic[curWall].Contains(oppositeDirDic[pair.Item2]))
                    {
                        var wall = 
                            BoxConfig.BoxConfigs.RandomItemWeighted(
                                x => canGoOutDirsDic[x.Walls].Contains(pair.Item2),
                                x => x.BasicWeight);
                        AddBox(pair.Item1, wall);
                        edgeLocStack.Push(pair.Item1);
                    }
                }
            }
        }
    }
}