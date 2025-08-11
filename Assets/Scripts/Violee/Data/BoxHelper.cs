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
            dirToVec2Dic = new Dictionary<EBoxDir, Vector2Int>()
            {
                { EBoxDir.Up, new Vector2Int(0, 1) },
                { EBoxDir.Down, new Vector2Int(0, -1) },
                { EBoxDir.Left, new Vector2Int(-1, 0) },
                { EBoxDir.Right, new Vector2Int(1, 0) }
            };
        }

        public const int BoxSize = 15;
        static float GetPointOffset() => Configer.SettingsConfig.BoxCostPosOffset;

        public static readonly List<EBoxDir> AllBoxDirs;
        public static readonly List<EWallType> AllWallTypes;
        
        /// <summary>
        /// (dir, oppositeDir)
        /// </summary>
        public static readonly Dictionary<EBoxDir, EBoxDir> OppositeDirDic;

        static readonly Dictionary<EBoxDir, Vector2Int> dirToVec2Dic;
        
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
            foreach (var dir in AllBoxDirs)
            {
                ret.Add((NextPos(thisPos, dir), OppositeDirDic[dir]));
            }
            return ret;
        }
        
        public static Vector3Int Pos2DTo3DBox(Vector2Int pos2D) 
            => new (pos2D.x * BoxSize, 0, pos2D.y * BoxSize);
        public static Vector2Int Pos3DTo2D(Vector3 pos3D) 
            => new ((int)((pos3D.x + BoxSize / 2f) / BoxSize), (int)((pos3D.z + BoxSize / 2f) / BoxSize));
        public static Vector3 Point3DOffsetInBox(EBoxDir dir)
            => new (dirToVec2Dic[dir].x * BoxSize * GetPointOffset(), 0, dirToVec2Dic[dir].y * BoxSize * GetPointOffset());
        public static Vector3 Pos2DTo3DPoint(Vector2Int pos2D, EBoxDir dir)
            => Pos2DTo3DBox(pos2D) + Point3DOffsetInBox(dir);
        public static Vector3 Pos2DTo3DEdge(Vector2Int pos2D, EBoxDir dir) 
            => Pos2DTo3DBox(pos2D) + 
               new Vector3(dirToVec2Dic[dir].x * BoxSize / 2f, 0, dirToVec2Dic[dir].y * BoxSize / 2f);
        
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
        public static bool HasSWallByByteAndDir(byte walls, EBoxDir dir) => (walls & (byte)dir) != 0;

        public static bool HasTiltWallByByte(byte walls) => (walls >> 4) != 0;

    }
}