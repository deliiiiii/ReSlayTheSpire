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
        public Vector3 Pos => 
            new (BelongBox.Pos.x + BoxHelper.dirToVec2Dic[Dir].x * offset, 
                BelongBox.Pos.y + BoxHelper.dirToVec2Dic[Dir].y * offset, 0);
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
        public BoxData()
        {
            InitPoint();
        }
        
        public Vector2Int Pos;
        public byte Walls;
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');
        public const int CrossWallCost = 1;
        public Sprite Sprite;
        [NotNull] public SerializableDictionary<EBoxDir, BoxPointData> PointDic;
        
        #region Path
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
        public int CostStraight(EBoxDir dir) => CanGoStraightWall(dir) ? 0 : CrossWallCost;
        public int CostTilt(EBoxDir from, EBoxDir to) => CanGoTiltWallBetween(from, to) ? 0 : CrossWallCost;
        #endregion
        
        
        #region GoCross
        public static bool CanGoStraightWall(byte walls, EBoxDir dir) => (walls & (byte)dir) == 0;
        bool CanGoStraightWall(EBoxDir dir) => (Walls & (byte)dir) == 0;

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