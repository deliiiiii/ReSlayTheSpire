using System;
using System.Reflection;

namespace BehaviourTree
{
    public interface IBoardKey
    {
        string Name { get; }
        // int Id { get; }
    }

    public class BoardKey<T> : IBoardKey, IEquatable<IBoardKey>, IComparable<IBoardKey>
    {
        public string Name { get; }
        // public int Id { get; }

        public BoardKey(string name)
        {
            Name = name;
        }
        public bool Equals(IBoardKey other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name == other.Name; // && Id == other.Id;
        }

        public int CompareTo(IBoardKey other)
        {
            if (other == null)
                return 1;
            return ReferenceEquals(this, other) ?
                0 : string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
        
        public Type GetKeyType()
        {
            return typeof(T);
        }
    }
}