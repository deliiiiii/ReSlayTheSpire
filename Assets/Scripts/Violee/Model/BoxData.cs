using System;
using System.Collections.Generic;
using System.Linq;
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

    public class BoxPointData
    {
        public EBoxDir Dir;
        public Observable<int> CostWall;
        public List<BoxPointData> NextPointInBoxList;
        public BoxData BelongBox;
        public void UpdateNextPointCost()
        {
            NextPointInBoxList.ForEach(nextPoint =>
            {
                nextPoint.CostWall.Value += 
                    BelongBox.CostTilt(Dir, nextPoint.Dir);
            });
        }
    }
    
    [Serializable]
    public class BoxData
    {
        static BoxData()
        {
            allDirs = (EBoxDir[])Enum.GetValues(typeof(EBoxDir));
        }
        
        public Vector2Int Pos;
        public byte Walls;
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');
        public const int CrossWallCost = 1;
        public Sprite Sprite;
        public Dictionary<EBoxDir, BoxPointData> PointDic;
        static EBoxDir[] allDirs;
        
        #region Path
        public void InitPoint(EBoxDir[] allBoxSides)
        {
            PointDic = new Dictionary<EBoxDir, BoxPointData>();
            allBoxSides.ForEach(dir =>
            {
                PointDic.Add(dir, new BoxPointData()
                {
                    Dir = dir,
                    BelongBox = this,
                    CostWall = new (int.MaxValue / 2),
                    NextPointInBoxList = new List<BoxPointData>()
                });
                allDirs.ForEach(dir2 =>
                {
                    if (dir == dir2)
                        return;
                    if (BoxHelper.oppositeDirDic[dir] == dir2)
                        return;
                    PointDic[dir].NextPointInBoxList.Add(PointDic[dir2]);
                });
            });
        }
        public int CostStraight(EBoxDir dir) => HasStraightWall(dir) ? 0 : CrossWallCost;
        public int CostTilt(EBoxDir from, EBoxDir to) => HasTiltWallBetween(from, to) ? 0 : CrossWallCost;
        #endregion
        
        
        #region GoCross
        public static bool HasStraightWall(byte walls, EBoxDir dir) => (walls & (byte)dir) == 0;
        bool HasStraightWall(EBoxDir dir) => (Walls & (byte)dir) == 0;

        /// <summary>
        /// dir1 dir2必须相邻！
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        bool HasTiltWallBetween(EBoxDir dir1, EBoxDir dir2)
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