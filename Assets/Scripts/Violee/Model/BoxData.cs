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
        public List<BoxPointData> NextPointList;
        public BoxData BelongBox;
        

        
        public void UpdateCostInBox()
        {
            NextPointList.ForEach(nextPoint =>
            {
                nextPoint.CostWall.Value += 
                    BelongBox.CanGoThroughFromToInside(Dir, nextPoint.Dir) ? 0 : 1;
            });
        }
    }
    
    [Serializable]
    public class BoxData
    {
        
        static EBoxDir[] allDirs;
        static BoxData()
        {
            allDirs = (EBoxDir[])Enum.GetValues(typeof(EBoxDir));
        }
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');

        public Vector2Int Pos;
        public byte Walls;
        public Sprite Sprite;
        public Dictionary<EBoxDir, BoxPointData> BoxPointDic;

        
        #region Path
        public void InitPoint(EBoxDir[] allBoxSides)
        {
            BoxPointDic = new Dictionary<EBoxDir, BoxPointData>();
            allBoxSides.ForEach(dir =>
            {
                BoxPointDic.Add(dir, new BoxPointData()
                {
                    Dir = dir,
                    BelongBox = this,
                    CostWall = new (int.MaxValue / 2),
                    NextPointList = new List<BoxPointData>()
                });
                allDirs.ForEach(dir2 =>
                {
                    if (dir == dir2)
                        return;
                    BoxPointDic[dir].NextPointList.Add(BoxPointDic[dir2]);
                });
            });
        }
        #endregion
        
        
        #region GoCross
        // public bool HasWallS1 => (Walls & 0b00000001) != 0;
        // public bool HasWallS2 => (Walls & 0b00000010) != 0;
        // public bool HasWallS4 => (Walls & 0b00000100) != 0;
        // public bool HasWallS8 => (Walls & 0b00001000) != 0;
        // public bool HasWallT12 => (Walls & 0b00010000) != 0;
        // public bool HasWallT24 => (Walls & 0b00100000) != 0;
        // public bool HasWallT48 => (Walls & 0b01000000) != 0;
        // public bool HasWallT81 => (Walls & 0b10000000) != 0;
        public static bool CanGoOutAt(byte walls, EBoxDir dir) => (walls & (byte)dir) == 0;
        bool ThisCanGoOutAt(EBoxDir dir) => (Walls & (byte)dir) == 0;

        public bool CanGoThroughFromToInside(EBoxDir dir1, EBoxDir dir2)
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
        bool CanGoThroughFromToOut(EBoxDir dir1, EBoxDir dir2)
        {
            return CanGoThroughFromToInside(dir1, dir2) && ThisCanGoOutAt(dir1) && ThisCanGoOutAt(dir2);
        }
        #endregion
    }
}