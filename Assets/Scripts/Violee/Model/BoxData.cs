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
    public struct Loc
    {
        public int X;
        public int Y;
    }
    
    [Serializable]
    public struct BoxData
    {
        public Loc Location;
        public byte Walls;
        public bool HasWallS1 => (Walls & 0b00000001) != 0;
        public bool HasWallS2 => (Walls & 0b00000010) != 0;
        public bool HasWallS4 => (Walls & 0b00000100) != 0;
        public bool HasWallS8 => (Walls & 0b00001000) != 0;
        public bool HasWallT12 => (Walls & 0b00010000) != 0;
        public bool HasWallT24 => (Walls & 0b00100000) != 0;
        public bool HasWallT48 => (Walls & 0b01000000) != 0;
        public bool HasWallT81 => (Walls & 0b10000000) != 0;
    }
}