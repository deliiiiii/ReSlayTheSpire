using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public enum EBoxSide
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    public class BoxPointData
    {
        public EBoxSide Dir;
        public int CostWall;
    }
    
    [Serializable]
    public class BoxData
    {
        static BoxData()
        {
            NextDirDic = new();
            var allDirs = (EBoxSide[])Enum.GetValues(typeof(EBoxSide));
            allDirs.ForEach(dir =>
            {
                NextDirDic.Add(dir, new List<EBoxSide>());
                allDirs.ForEach(dir2 =>
                {
                    if (dir == dir2)
                        return;
                    NextDirDic[dir].Add(dir2);
                });
            });
        }
        public BoxData(byte walls, Sprite sprite)
        {
            Walls = walls;
            Sprite = sprite;
        }
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');
        
        public byte Walls;
        public Sprite Sprite;
        public Dictionary<EBoxSide, BoxPointData> BoxPointDic;
        
        public bool HasWallS1 => (Walls & 0b00000001) != 0;
        public bool HasWallS2 => (Walls & 0b00000010) != 0;
        public bool HasWallS4 => (Walls & 0b00000100) != 0;
        public bool HasWallS8 => (Walls & 0b00001000) != 0;
        public bool HasWallT12 => (Walls & 0b00010000) != 0;
        public bool HasWallT24 => (Walls & 0b00100000) != 0;
        public bool HasWallT48 => (Walls & 0b01000000) != 0;
        public bool HasWallT81 => (Walls & 0b10000000) != 0;

        
        #region Path

        public static Dictionary<EBoxSide, List<EBoxSide>> NextDirDic;
        public void ResetCost(EBoxSide[] allBoxSides)
        {
            BoxPointDic = new Dictionary<EBoxSide, BoxPointData>();
            allBoxSides.ForEach(dir => BoxPointDic.Add(dir, new BoxPointData(){CostWall = int.MaxValue / 2}));
        }
        #endregion
        
        
        #region GoCross
        public static bool CanGoOutAt(byte walls, EBoxSide dir) => (walls & (byte)dir) == 0;
        bool ThisCanGoOutAt(EBoxSide dir) => (Walls & (byte)dir) == 0;

        public bool CanGoThroughFromToInside(EBoxSide dir1, EBoxSide dir2)
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
        bool CanGoThroughFromToOut(EBoxSide dir1, EBoxSide dir2)
        {
            return CanGoThroughFromToInside(dir1, dir2) && ThisCanGoOutAt(dir1) && ThisCanGoOutAt(dir2);
        }
        #endregion
    }
}