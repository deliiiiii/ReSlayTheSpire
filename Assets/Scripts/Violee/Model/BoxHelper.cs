using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Violee
{
    public static class BoxHelper
    {
        static BoxHelper()
        {
            EmptyBoxConfig = BoxConfigList.First(x => x.Walls == 0);
            AllBoxDirs = new List<EBoxDir>();
            var array = Enum.GetValues(typeof(EBoxDir));
            for (int i = 0; i < array.Length; i++)
            {
                AllBoxDirs.Add((EBoxDir)array.GetValue(i));
            }
            AllWallTypes = new List<EWallType>();
            array = Enum.GetValues(typeof(EWallType));
            for (int i = 0; i < array.Length; i++)
            {
                AllWallTypes.Add((EWallType)array.GetValue(i));
            }
            OppositeDirDic = new Dictionary<EBoxDir, EBoxDir>()
            {
                { EBoxDir.Up, EBoxDir.Down },
                { EBoxDir.Down, EBoxDir.Up },
                { EBoxDir.Left, EBoxDir.Right },
                { EBoxDir.Right, EBoxDir.Left }
            };
            DirToVec2Dic = new Dictionary<EBoxDir, Vector2Int>()
            {
                { EBoxDir.Up, new Vector2Int(0, 1) },
                { EBoxDir.Down, new Vector2Int(0, -1) },
                { EBoxDir.Left, new Vector2Int(-1, 0) },
                { EBoxDir.Right, new Vector2Int(1, 0) }
            };
        }

        public const float BoxSize = 10;
        static List<BoxConfigSingle> BoxConfigList => Configer.BoxConfig.BoxConfigList;
        static float pointOffset => Configer.SettingsConfig.BoxCostPosOffset;
        public static readonly BoxConfigSingle EmptyBoxConfig;
        public static readonly List<EBoxDir> AllBoxDirs;
        public static readonly List<EWallType> AllWallTypes;
        
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        public static readonly Dictionary<EBoxDir, EBoxDir> OppositeDirDic;

        static readonly Dictionary<EBoxDir, Vector2Int> DirToVec2Dic;
        
        /// <param name="thisLoc">(1, 1)</param>
        /// <param name="dir">Up</param>
        /// <returns>(1, 2)</returns>
        public static Vector2Int NextPos(Vector2Int thisLoc, EBoxDir dir)
        {
            return new Vector2Int(thisLoc.x + DirToVec2Dic[dir].x, thisLoc.y + DirToVec2Dic[dir].y);
        }
        
        /// <param name="thisPos">(1, 1)</param>
        /// <returns>List: ((1, 2), Up), ((2, 1), Right), ((1, 0), Down), ((0, 1), Left)</returns>
        public static List<(Vector2Int, EBoxDir)> GetNextLocAndGoInDirList(Vector2Int thisPos)
        {
            var ret = new List<(Vector2Int, EBoxDir)>();
            foreach (var dir in AllBoxDirs)
            {
                ret.Add((NextPos(thisPos, dir), OppositeDirDic[dir]));
            }
            return ret;
        }
        
        public static Vector3 Pos2DTo3DBox(Vector2 pos2D) => new (pos2D.x * BoxSize, 0, pos2D.y * BoxSize);
        public static Vector3 Pos2DTo3DPoint(Vector2 pos2D, EBoxDir dir) =>
            Pos2DTo3DBox(pos2D) + new Vector3(DirToVec2Dic[dir].x * BoxSize * pointOffset, 0, DirToVec2Dic[dir].y * BoxSize * pointOffset);
        public static Vector3 Pos2DTo3DEdge(Vector2 pos2D, EBoxDir dir) =>
            Pos2DTo3DBox(pos2D) + new Vector3(DirToVec2Dic[dir].x * BoxSize / 2, 0, DirToVec2Dic[dir].y * BoxSize / 2);
        public static Vector2Int Pos3DTo2D(Vector3 pos3D) => 
            new ((int)((pos3D.x + BoxSize / 2) / BoxSize), (int)((pos3D.z + BoxSize / 2) / BoxSize));
        
        public static EWallType WallDirToType(EBoxDir dir)
        {
            return dir switch
            {
                EBoxDir.Up => EWallType.S1,
                EBoxDir.Right => EWallType.S2,
                EBoxDir.Down => EWallType.S4,
                EBoxDir.Left => EWallType.S8,
                _ => throw new ArgumentException($"Invalid direction: {dir}")
            };
        }
    }
}