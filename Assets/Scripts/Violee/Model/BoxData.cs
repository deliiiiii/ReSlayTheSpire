using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
    
    [Serializable]
    public class BoxData
    {
        public BoxData(byte walls, Sprite sprite)
        {
            Walls = walls;
            Sprite = sprite;
        }
        [ShowInInspector]
        string WallsInBinary => Convert.ToString(Walls, 2).PadLeft(8, '0');
        
        public byte Walls;
        public Sprite Sprite;
        public Dictionary<EBoxSide, int> CostWallDic;
        
        public bool HasWallS1 => (Walls & 0b00000001) != 0;
        public bool HasWallS2 => (Walls & 0b00000010) != 0;
        public bool HasWallS4 => (Walls & 0b00000100) != 0;
        public bool HasWallS8 => (Walls & 0b00001000) != 0;
        public bool HasWallT12 => (Walls & 0b00010000) != 0;
        public bool HasWallT24 => (Walls & 0b00100000) != 0;
        public bool HasWallT48 => (Walls & 0b01000000) != 0;
        public bool HasWallT81 => (Walls & 0b10000000) != 0;
        
        public static bool CanGoOutAt(byte walls, EBoxSide dir) => CanGoOutAt(walls, (byte)dir);
        static bool CanGoOutAt(byte walls, byte dir) => (walls & dir) == 0;
        bool ThisCanGoOutAt(byte dir) => CanGoOutAt(Walls, dir);

        public bool CanGoThroughFromToInside(byte dir1, byte dir2)
        {
            var big = dir1 > dir2 ? dir1 : dir2;
            var small = dir1 < dir2 ? dir1 : dir2;
            var tWalls = Walls >> 4;
            var from = small;
            if (big == 8 && small == 1)
                from = 8;
            return (big, small) switch
            {
                (4, 1) => (tWalls & 3) != 3
                          && (tWalls & 12) != 12
                          && (tWalls & 5) != 5
                          && (tWalls & 10) != 10,
                (8, 2) => (tWalls & 9) != 9
                          && (tWalls & 6) != 6
                          && (tWalls & 5) != 5
                          && (tWalls & 10) != 10,
                _ => tWalls == from || (tWalls | from) != tWalls
            };
        }
        bool CanGoThroughFromToOut(byte dir1, byte dir2)
        {
            return CanGoThroughFromToInside(dir1, dir2) && ThisCanGoOutAt(dir1) && ThisCanGoOutAt(dir2);
        }
    }
}