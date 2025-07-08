using System;
using System.Reflection;
using UnityEngine;

namespace BehaviourTree
{
    public interface IBoardValue<out T> where T : IComparable
    {
        string Name { get; }
        T Value { get; }
        // int Id { get; }
    }
    [Serializable]
    public class BoardValue<T> : IBoardValue<T>, IEquatable<IBoardValue<T>>, IComparable<IBoardValue<T>> where T : IComparable
    {
        [SerializeField]
        string name;
        public string Name
        {
            get => name;
            private set => name = value;
        }
        [SerializeField]
        T _value;
        public T Value
        {
            get => _value;
            private set => _value = value;
        }
        // public int Id { get; }

        public BoardValue(string name)
        {
            Name = name;
        }
        public bool Equals(IBoardValue<T> other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name == other.Name; // && Id == other.Id;
        }

        public int CompareTo(IBoardValue<T> other)
        {
            if (other == null)
                return 0;
            return ReferenceEquals(this, other) ? 0 : Value.CompareTo(other.Value);
        }
        
        public Type GetKeyType()
        {
            return typeof(T);
        }
    }
}