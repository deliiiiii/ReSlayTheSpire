using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public static class BoxHelper
    {
        static BoxHelper()
        {
            emptyBoxConfig = BoxConfigs.First(x => x.Walls == 0);
            allBoxWalls = BoxConfigs.Select(x => x.Walls).Distinct().ToList();
            allBoxSides = (EBoxDir[])Enum.GetValues(typeof(EBoxDir));
            
            canGoOutDirsDic = new Dictionary<byte, List<EBoxDir>>();
            allBoxWalls.ForEach(w =>
            {
                canGoOutDirsDic.Add(w, new List<EBoxDir>());
                allBoxSides.ForEach(dir =>
                {
                    if (BoxData.HasStraightWall(w, dir))
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
            dirToVec2Dic = new Dictionary<EBoxDir, Vector2Int>()
            {
                { EBoxDir.Up, new Vector2Int(0, 1) },
                { EBoxDir.Down, new Vector2Int(0, -1) },
                { EBoxDir.Left, new Vector2Int(-1, 0) },
                { EBoxDir.Right, new Vector2Int(1, 0) }
            };
        }
        static List<BoxConfigSingle> BoxConfigs => Configer.BoxConfigList;
        public static BoxConfigSingle emptyBoxConfig;
        public static List<byte> allBoxWalls;
        public static EBoxDir[] allBoxSides;
        /// <summary>
        /// (walls, [outDir1, ...])
        /// </summary>
        public static Dictionary<byte, List<EBoxDir>> canGoOutDirsDic;
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        public static Dictionary<EBoxDir, EBoxDir> oppositeDirDic;

        public static Dictionary<EBoxDir, Vector2Int> dirToVec2Dic;
        
        /// <param name="thisLoc">(1, 1)</param>
        /// <param name="dir">Up</param>
        /// <returns>(1, 2)</returns>
        public static Vector2Int NextPos(Vector2Int thisLoc, EBoxDir dir)
        {
            return new Vector2Int(thisLoc.x + dirToVec2Dic[dir].x, thisLoc.y + dirToVec2Dic[dir].y);
        }
        
        /// <param name="thisPos">(1, 1)</param>
        /// <returns>List: ((1, 2), Up), ((2, 1), Right), ((1, 0), Down), ((0, 1), Left)</returns>
        public static List<(Vector2Int, EBoxDir)> GetNextLocAndDirList(Vector2Int thisPos)
        {
            var ret = new List<(Vector2Int, EBoxDir)>();
            allBoxSides.ForEach(dir =>
            {
                ret.Add((NextPos(thisPos, dir), oppositeDirDic[dir]));
            });
            return ret;
        }
    }
}