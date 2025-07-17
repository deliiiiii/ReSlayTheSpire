using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public enum EBoxDir
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    public enum EWallType
    {
        S1,
        S2,
        S4,
        S8,
        T1248,
        T2481,
    }

    [Serializable]
    public class BoxPointData: IComparable<BoxPointData>, IEquatable<BoxPointData>
    {
        public EBoxDir Dir;
        public Observable<int> CostWall;
        public Observable<int> CostStep;
        [HideInInspector]
        public List<BoxPointData> NextPointsInBox;
        [HideInInspector]
        public BoxData BelongBox;
        public Vector2 Pos2D => 
            new (BelongBox.Pos.x + BoxHelper.dirToVec2Dic[Dir].x * offset, 
                BelongBox.Pos.y + BoxHelper.dirToVec2Dic[Dir].y * offset);
        public void UpdateNextPointCost()
        {
            foreach (var nextPoint in NextPointsInBox)
            {
                nextPoint.CostWall.Value = Math.Min(
                    nextPoint.CostWall.Value,
                    CostWall + BelongBox.CostTilt(Dir, nextPoint.Dir));
            }
        }

        static float offset => Configer.Instance.SettingsConfig.BoxCostPosOffset;
        public int CompareTo(BoxPointData other)
        {
            if (BelongBox != other.BelongBox)
                return -1;
            return Dir.CompareTo(other.Dir);
        }

        public bool Equals(BoxPointData other)
        {
            return CompareTo(other) == 0;
        }
    }
    
    [Serializable]
    public class BoxData
    {
        public Vector2Int Pos;
        // // TODO 1：之后在sprite上自己划线？ 2：拿预制体的3d模型
        // public Sprite Sprite;
        public event Action<EWallType> OnAddWall;
        public event Action<EWallType> OnRemoveWall;
        #region Walls
        public byte Walls;
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');

        EWallType WallDirToType(EBoxDir dir)
        {
            return dir switch
            {
                EBoxDir.Up => EWallType.S1,
                EBoxDir.Right => EWallType.S2,
                EBoxDir.Down => EWallType.S4,
                EBoxDir.Left => EWallType.S8,
            };
        }
        public static bool HasWallByDir(byte walls, EBoxDir dir) => (walls & (byte)dir) != 0;
        public bool HasWallByDir(EBoxDir dir) => (Walls & (byte)dir) != 0;
        public bool HasWallByType(EWallType t)
        {
            return t switch
            {
                EWallType.S1 => (Walls & (1 << 0)) != 0,
                EWallType.S2 => (Walls & (1 << 1)) != 0,
                EWallType.S4 => (Walls & (1 << 2)) != 0,
                EWallType.S8 => (Walls & (1 << 3)) != 0,
                EWallType.T1248 => (Walls & (1 << 4)) != 0 && (Walls & (1 << 6)) != 0,
                EWallType.T2481 => (Walls & (1 << 5)) != 0 && (Walls & (1 << 7)) != 0,
            };
        }
        public void AddSWallByDir(EBoxDir dir)
        {
            Walls = dir switch
            {
                EBoxDir.Up => (byte)(Walls | (1 << 0)),
                EBoxDir.Right => (byte)(Walls | (1 << 1)),
                EBoxDir.Down => (byte)(Walls | (1 << 2)),
                EBoxDir.Left => (byte)(Walls | (1 << 3)),
            };
            OnAddWall?.Invoke(WallDirToType(dir));
        }
        public void RemoveSWallByDir(EBoxDir dir)
        {
            Walls = dir switch
            {
                EBoxDir.Up =>     (byte)(Walls & ~(1 << 0)),
                EBoxDir.Right =>  (byte)(Walls & ~(1 << 1)),
                EBoxDir.Down =>   (byte)(Walls & ~(1 << 2)),
                EBoxDir.Left =>   (byte)(Walls & ~(1 << 3)),
            };
            OnRemoveWall?.Invoke(WallDirToType(dir));
        }
        #endregion
        
        
        #region Path
        public const int CrossWallCost = 1;
        public SerializableDictionary<EBoxDir, BoxPointData> PointDic;
        public void InitPoint()
        {
            PointDic = new SerializableDictionary<EBoxDir, BoxPointData>();
            foreach (var dir in BoxHelper.allBoxDirs)
            {
                PointDic.Add(dir, new BoxPointData()
                {
                    Dir = dir,
                    BelongBox = this,
                    CostWall = new (int.MaxValue / 2),
                    NextPointsInBox = new List<BoxPointData>()
                });
            }

            foreach (var dir in BoxHelper.allBoxDirs)
            {
                foreach (var dir2 in BoxHelper.allBoxDirs)
                {
                    if (dir == dir2)
                        continue;
                    if (BoxHelper.oppositeDirDic[dir] == dir2)
                        continue;
                    PointDic[dir].NextPointsInBox.Add(PointDic[dir2]);
                }
            }
            
        }
        public int CostStraight(EBoxDir dir) => HasWallByDir(dir) ? CrossWallCost : 0;
        public int CostTilt(EBoxDir from, EBoxDir to) => CanGoTiltWallBetween(from, to) ? 0 : CrossWallCost;
        
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
            var x = Walls >> 4;
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
                _ => x == fromDif || (x | fromDif) != x
            };
        }
        #endregion
    }
}