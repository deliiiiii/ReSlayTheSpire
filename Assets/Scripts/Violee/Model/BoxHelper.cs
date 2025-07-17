using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee
{
    // [InitializeOnLoad]
    public static class BoxHelper
    {
        public static void GetSth()
        {
            
        }
        static BoxHelper()
        {
            emptyBoxConfig = BoxConfigList.First(x => x.Walls == 0);
            allBoxWalls = BoxConfigList.Select(x => x.Walls).Distinct().ToList();
            allBoxDirs = new List<EBoxDir>();
            var array = Enum.GetValues(typeof(EBoxDir));
            for (int i = 0; i < array.Length; i++)
            {
                allBoxDirs.Add((EBoxDir)array.GetValue(i));
            }
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
        static List<BoxConfigSingle> BoxConfigList => Configer.Instance.BoxConfig.BoxConfigList;
        public static BoxConfigSingle emptyBoxConfig;
        public static List<byte> allBoxWalls;
        public static List<EBoxDir> allBoxDirs;
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
        public static List<(Vector2Int, EBoxDir)> GetNextLocAndGoInDirList(Vector2Int thisPos)
        {
            var ret = new List<(Vector2Int, EBoxDir)>();
            foreach (var dir in allBoxDirs)
            {
                ret.Add((NextPos(thisPos, dir), oppositeDirDic[dir]));
            }
            return ret;
        }
        
        public static Vector3 Pos2DTo3D(Vector2 pos2D) => new (pos2D.x * 10f, 0, pos2D.y * 10f);
    }
}