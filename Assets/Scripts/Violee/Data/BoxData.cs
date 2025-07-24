using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Violee
{
    public enum EBoxDir : byte
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    public enum EWallType : byte
    {
        S1 = 1 << 0,
        S2 = 1 << 1,
        S4 = 1 << 2,
        S8 = 1 << 3,
        T1248 = 1 << 4 | 1 << 6,
        T2481 = 1 << 5 | 1 << 7,
    }
    
    [Serializable]
    public class BoxData
    {
        public BoxData(Vector2Int pos, BoxConfigSingle config)
        {
            Pos2D = pos;
            wallsByte = config.Walls;
            foreach (var wallType in BoxHelper.AllWallTypes)
            {
                WallKList.Add(WallData.Create(wallType, EDoorType.Random));
                if ((wallsByte & (int)wallType) == (int)wallType)
                    WallKList[wallType].HasWall = true;
            }
            PointKList = new MyKeyedCollection<EBoxDir, BoxPointData>(b => b.Dir);
            foreach (var dir in BoxHelper.AllBoxDirs)
            {
                PointKList.Add(new BoxPointData()
                {
                    BelongBox = this,
                    Dir = dir,
                    CostWall = new (int.MaxValue / 2),
                    Visited = new(false),
                    Pos3D = BoxHelper.Pos2DTo3DPoint(Pos2D, dir),
                    NextPointsInBox = new List<BoxPointData>()
                });
            }
            foreach (var dir in BoxHelper.AllBoxDirs)
            {
                foreach (var dir2 in BoxHelper.AllBoxDirs)
                {
                    if (dir == dir2)
                        continue;
                    if (BoxHelper.OppositeDirDic[dir] == dir2)
                        continue;
                    PointKList[dir].NextPointsInBox.Add(PointKList[dir2]);
                }
            }
        }
        
        public Vector2Int Pos2D;
        #region Walls
        [NonSerialized]
        byte wallsByte;
        public event Action<WallData> OnWallDataChanged;

        public readonly MyKeyedCollection<EWallType, WallData> WallKList = new(w => w.WallType);
        
        public static bool HasSWallByByteAndDir(byte walls, EBoxDir dir) => (walls & (byte)dir) != 0;

        public bool HasSWallByDir(EBoxDir dir, out WallData wallData)
        {
            wallData = WallKList[BoxHelper.WallDirToType(dir)];
            return wallData.HasWall;
        }
        public void AddSWall(WallData wallData)
        {
            wallsByte |= (byte)wallData.WallType;
            wallData.HasWall = true;
            OnWallDataChanged?.Invoke(wallData);
        }
        public void RemoveSWall(EBoxDir dir)
        {
            RemoveSWall(BoxHelper.WallDirToType(dir));
        }
        void RemoveSWall(EWallType wallType)
        {
            wallsByte &= (byte)~wallType;
            WallKList[wallType].HasWall = false;
            OnWallDataChanged?.Invoke(WallKList[wallType]);
        }
        #endregion
        
        
        #region Path
        const int WallCost = 10;
        const int DoorCost = 1;
        public MyKeyedCollection<EBoxDir, BoxPointData> PointKList;

        public void ResetCost()
        {
            foreach (var pointData in PointKList)
            {
                pointData.CostWall.Value = int.MaxValue / 2;
                pointData.Visited.Value = false;
            }
        }

        public int CostStraight(EBoxDir dir)
        {
            if (!HasSWallByDir(dir, out var wallData))
                return 0;
            if(wallData.DoorType == EDoorType.Wooden)
                return DoorCost;
            return WallCost;
        }

        public int CostTilt(EBoxDir from, EBoxDir to)
        {
            if (CanGoTiltWallBetween(from, to))
                return 0;
            var t = (from, to) switch
            {
                (EBoxDir.Up, EBoxDir.Right)
                    or (EBoxDir.Right, EBoxDir.Up)
                    or (EBoxDir.Down, EBoxDir.Left)
                    or (EBoxDir.Left, EBoxDir.Down) => EWallType.T1248,
                (EBoxDir.Up, EBoxDir.Left)
                    or (EBoxDir.Left, EBoxDir.Up)
                    or (EBoxDir.Down, EBoxDir.Right)
                    or (EBoxDir.Right, EBoxDir.Down) => EWallType.T2481,
                _ => throw new ArgumentException($"from{from} and to{to} must be adjacent directions!"),
            };
            if(WallKList[t].DoorType == EDoorType.Wooden)
                return DoorCost;
            return WallCost;
        }
        
        /// <summary>
        /// dir1 dir2必须相邻！
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        bool CanGoTiltWallBetween(EBoxDir dir1, EBoxDir dir2)
        {
            var bigDir = dir1 > dir2 ? dir1 : dir2;
            var smallDif = dir1 < dir2 ? dir1 : dir2;
            var big = (byte)bigDir;
            var small = (byte)smallDif;
            // tilt walls
            var x = wallsByte >> 4;
            var fromDif = small;
            if (big == 8 && small == 1)
                fromDif = 8;
            return (big, small) switch
            {
                (4, 1) => (x & 3) != 3
                          && (x & 12) != 12
                          && (x & 5) != 5
                          && (x & 10) != 10,
                (8, 2) => (x & 9) != 9
                          && (x & 6) != 6
                          && (x & 5) != 5
                          && (x & 10) != 10,
                _ => x == 0 || x == fromDif || (x | fromDif) != x
            };
        }
        #endregion
    }
}