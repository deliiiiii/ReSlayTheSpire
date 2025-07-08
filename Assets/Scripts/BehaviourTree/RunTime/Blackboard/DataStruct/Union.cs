using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public enum EBoardEValueType : byte
    {
        Undefined = 0, // 表示Key不存在
        @null = 1, // 表示Key存在，但值为null
        @int = 2,
        @long = 3,
        @float = 4,
        @double = 5,
        @bool = 6,
        @Vector3 = 7,
        // ...
        @object = 15, // 引用类型
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Union : IEquatable<Union>, IComparable<Union>
    {
        [FieldOffset(0)] public EBoardEValueType BoardEValueType;
        [FieldOffset(1)] public int intVal;
        [FieldOffset(1)] public long longVal;
        [FieldOffset(1)] public float floatVal;    
        [FieldOffset(1)] public double doubleVal;
        [FieldOffset(1)] public bool boolVal;
        [FieldOffset(1)] public Vector3 vector3; // sizeof => 12
        // ...
        [FieldOffset(16)] [CanBeNull] public object objectVal; // 内存对齐后需要偏移16
        public bool Equals(Union other)
        {
            if (BoardEValueType != other.BoardEValueType)
                return false;

            return BoardEValueType switch
            {
                EBoardEValueType.@int      => intVal == other.intVal,
                EBoardEValueType.@long     => longVal == other.longVal,
                EBoardEValueType.@float    => Math.Abs(floatVal - other.floatVal) < 0.0001f,
                EBoardEValueType.@double   => Math.Abs(doubleVal - other.doubleVal) < 0.0001,
                EBoardEValueType.@bool     => boolVal == other.boolVal,
                EBoardEValueType.@Vector3  => vector3.Equals(other.vector3),
                EBoardEValueType.@object   => ReferenceEquals(objectVal, other.objectVal),
                _                          => true
            };
        }
        
        public int CompareTo(Union other)
        {
            if (BoardEValueType != other.BoardEValueType)
                return 0;
            
            return BoardEValueType switch
            {
                EBoardEValueType.@int      => intVal.CompareTo(other.intVal),
                EBoardEValueType.@long     => longVal.CompareTo(other.longVal),
                EBoardEValueType.@float    => floatVal.CompareTo(other.floatVal),
                EBoardEValueType.@double   => doubleVal.CompareTo(other.doubleVal),
                EBoardEValueType.@bool     => boolVal.CompareTo(other.boolVal),
                EBoardEValueType.@Vector3  => vector3.magnitude.CompareTo(other.vector3.magnitude),
                EBoardEValueType.@object   => 0,
                _                          => 0,
            };
        }

        public void SetType(Type t)
        {
            if (t == typeof(int))
                BoardEValueType = EBoardEValueType.@int;
            else if (t == typeof(long))
                BoardEValueType = EBoardEValueType.@long;
            else if (t == typeof(float))
                BoardEValueType = EBoardEValueType.@float;
            else if (t == typeof(double))
                BoardEValueType = EBoardEValueType.@double;
            else if (t == typeof(bool))
                BoardEValueType = EBoardEValueType.@bool;
            else if (t == typeof(Vector3))
                BoardEValueType = EBoardEValueType.@Vector3;
            else if (t == typeof(object))
                BoardEValueType = EBoardEValueType.@object;
            else
            {
                MyDebug.LogError($"Unexpected type {t}");
                BoardEValueType = EBoardEValueType.Undefined;
            }
        }
    }
}