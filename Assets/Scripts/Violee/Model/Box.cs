using System;
using Sirenix.OdinInspector;

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
    public class Box
    {
        [ShowInInspector]
        byte walls;
        public bool HasWallS1 => (walls & 0b00000001) != 0;
        public bool HasWallS2 => (walls & 0b00000010) != 0;
        public bool HasWallS4 => (walls & 0b00000100) != 0;
        public bool HasWallS8 => (walls & 0b00001000) != 0;
        public bool HasWallT12 => (walls & 0b00010000) != 0;
        public bool HasWallT24 => (walls & 0b00100000) != 0;
        public bool HasWallT48 => (walls & 0b01000000) != 0;
        public bool HasWallT81 => (walls & 0b10000000) != 0;

        public static bool IsConnect(Box b, EBoxSide s1, EBoxSide s2)
        {
            return IsConnect(b, (byte)s1, (byte)s2);
        }
        static bool IsConnect(Box b,byte s1, byte s2)
        {
            var big = s1 > s2 ? s1 : s2;
            var small = s1 < s2 ? s1 : s2;
            var x = b.walls & 0b1111;
            var y = b.walls >> 4;
            var from = small;
            if (big == 8 && small == 1)
                from = 8;
            var sIsConnect = ((x & s1) | (x & s2)) == 0;
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