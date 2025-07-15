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
        
        public bool HasWallS1 => (Walls & 0b00000001) != 0;
        public bool HasWallS2 => (Walls & 0b00000010) != 0;
        public bool HasWallS4 => (Walls & 0b00000100) != 0;
        public bool HasWallS8 => (Walls & 0b00001000) != 0;
        public bool HasWallT12 => (Walls & 0b00010000) != 0;
        public bool HasWallT24 => (Walls & 0b00100000) != 0;
        public bool HasWallT48 => (Walls & 0b01000000) != 0;
        public bool HasWallT81 => (Walls & 0b10000000) != 0;
        
        public static bool CanGoOutAt(byte walls, EBoxSide dir)
        {
            return (walls | (byte)dir) != walls;
        }
        public bool ThisCanGoThroughFromTo(byte dir1, byte dir2)
        {
            var big = dir1 > dir2 ? dir1 : dir2;
            var small = dir1 < dir2 ? dir1 : dir2;
            var x = Walls & 0b1111;
            var y = Walls >> 4;
            var from = small;
            if (big == 8 && small == 1)
                from = 8;
            var sIsConnect = ((x & dir1) | (x & dir2)) == 0;
            var tIsConnect = (big, small) switch
            {
                (4, 1) => (y & 3) != 3
                          && (y & 12) != 12
                          && (y & 5) != 5
                          && (y & 10) != 10,
                (8, 2) => (y & 9) != 9
                          && (y & 6) != 6
                          && (y & 5) != 5
                          && (y & 10) != 10,
                _ => y == from || (y | from) != y,
            };
            return sIsConnect && tIsConnect;
        }
    }
}