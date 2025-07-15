using System;
using System.Collections.Generic;

namespace Violee
{
    public readonly struct Loc : IComparable<Loc>
    {
        public readonly int X;
        public readonly int Y;

        public Loc(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Loc other)
        {
            if (X != other.X)
            {
                return X - other.X;
            }
            return Y - other.Y;
        }

        public override string ToString()
        {
            return $"Loc({X}, {Y})";
        }
    }
    [Serializable]
    public class MapData
    {
        public SerializableDictionary<Loc, BoxData> BoxDic;
    }
}