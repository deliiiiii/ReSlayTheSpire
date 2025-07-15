using System;
using System.Collections.Generic;

namespace Violee
{
    public readonly struct Loc : IComparable<Loc>, IEquatable<Loc>
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

        public bool Equals(Loc other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Loc other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
    [Serializable]
    public class MapData
    {
        public SerializableDictionary<Loc, BoxData> BoxDic;
    }
}