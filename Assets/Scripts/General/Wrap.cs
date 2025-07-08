using System;
using UnityEngine;
namespace General
{
    [Serializable]
    public class Wrap<T> : IComparable<Wrap<T>> where T : IComparable<T>
    {
        [SerializeField]
        T value;
        public Wrap(T initValue)
        {
            value = initValue;
        }
        public static implicit operator T(Wrap<T> v)
        {
            return v.value;
        }
        
        public static implicit operator float(Wrap<T> v)
        {
            return v.value switch
            {
                int i => i,
                float f => f,
                double d => (float)d,
                char c => c,
                _ => (dynamic)v.value
            };
        }
    
        public override string ToString()
        {
            return value.ToString();
        }
        
        public int CompareTo(Wrap<T> other)
        {
            if (other == null)
                return 1;
            if (ReferenceEquals(this, other))
                return 0;
            return value.CompareTo(other.value);
        }
    }
}