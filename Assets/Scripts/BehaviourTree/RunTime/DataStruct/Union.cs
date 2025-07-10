using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public enum EBoardEValueType : byte
    {
        @null = 1,
        @int = 2,
        @long = 3,
        @float = 4,
        @double = 5,
        @bool = 6,
        @string = 7,
        @Vector3 = 14,
        @object = 15, // 引用类型
    }
    
    [Serializable]
    public class Union : IComparable<Union>
    {
        public EBoardEValueType BoardEValueType;
        [ShowIf(nameof(isInt))][OnValueChanged(nameof(OnValueChanged))]
        public int intVal; bool isInt => BoardEValueType == EBoardEValueType.@int;
        [ShowIf(nameof(isLong))][OnValueChanged(nameof(OnValueChanged))]
        public long longVal; bool isLong => BoardEValueType == EBoardEValueType.@long;
        [ShowIf(nameof(isFloat))][OnValueChanged(nameof(OnValueChanged))]
        public float floatVal; bool isFloat => BoardEValueType == EBoardEValueType.@float;  
        [ShowIf(nameof(isDouble))][OnValueChanged(nameof(OnValueChanged))]
        public double doubleVal; bool isDouble => BoardEValueType == EBoardEValueType.@double;
        [ShowIf(nameof(isBool))][OnValueChanged(nameof(OnValueChanged))]
        public bool boolVal; bool isBool => BoardEValueType == EBoardEValueType.@bool;
        [ShowIf(nameof(isString))][OnValueChanged(nameof(OnValueChanged))]
        public string stringVal; bool isString => BoardEValueType == EBoardEValueType.@string;
        [ShowIf(nameof(isVector3))][OnValueChanged(nameof(OnValueChanged))]
        public Vector3 vector3; bool isVector3 => BoardEValueType == EBoardEValueType.@Vector3;  
        [CanBeNull] [ShowIf(nameof(isObject))]
        public object objectVal; bool isObject => BoardEValueType == EBoardEValueType.@object;

        void OnValueChanged()
        {
            if(isInt)
                objectVal = intVal;
            else if(isLong)
                objectVal = longVal;
            else if(isFloat)
                objectVal = floatVal;
            else if(isDouble)
                objectVal = doubleVal;
            else if(isBool)
                objectVal = boolVal;
            else if(isString)
                objectVal = stringVal;
            else if(isVector3)
                objectVal = vector3;
            
            MyDebug.Log($"onValueChanged: {BoardEValueType} -> {objectVal}");
            // else if(isObject)
            //     objectVal = objectVal;
            // else
            //     objectVal = null;
        }
        public int CompareTo(Union other)
        {
            if (BoardEValueType != other.BoardEValueType)
            {
                MyDebug.LogWarning($"Cannot compare Union different types: {BoardEValueType} vs {other.BoardEValueType}");
            }
            
            return BoardEValueType switch
            {
                EBoardEValueType.@int      => intVal.CompareTo(other.intVal),
                EBoardEValueType.@long     => longVal.CompareTo(other.longVal),
                EBoardEValueType.@float    => floatVal.CompareTo(other.floatVal),
                EBoardEValueType.@double   => doubleVal.CompareTo(other.doubleVal),
                EBoardEValueType.@bool     => boolVal.CompareTo(other.boolVal),
                EBoardEValueType.@string   => string.Compare(stringVal, other.stringVal, StringComparison.Ordinal),
                EBoardEValueType.@Vector3  => vector3.magnitude.CompareTo(other.vector3.magnitude),
                EBoardEValueType.@object   => 0,
                _                          => 0,
            };
        }

        public static Union Null = Create(null, null);
        public static Union Create(Type t, object v)
        {
            // MyDebug.Log("Create Union with type: " + t?.Name + ", value: " + v);
            var ret = new Union
            {
                BoardEValueType = EBoardEValueType.@null
            };
            if (t == null || v == null)
            {
                return ret;
            }
            
            if (t == typeof(int))
            {
                ret.BoardEValueType = EBoardEValueType.@int;
                ret.intVal = (int)v;
            }
            else if (t == typeof(long))
            {
                ret.BoardEValueType = EBoardEValueType.@long;
                ret.longVal = (long)v;
            }
            else if (t == typeof(float))
            {
                ret.BoardEValueType = EBoardEValueType.@float;
                ret.floatVal = (float)v;
            }
            else if (t == typeof(double))
            {
                ret.BoardEValueType = EBoardEValueType.@double;
                ret.doubleVal = (double)v;
            }
            else if (t == typeof(bool))
            {
                ret.BoardEValueType = EBoardEValueType.@bool;
                ret.boolVal = (bool)v;
            }
            else if (t == typeof(Vector3))
            {
                ret.BoardEValueType = EBoardEValueType.@Vector3;
                ret.vector3 = (Vector3)v;
            }
            else if (t == typeof(string))
            {
                ret.BoardEValueType = EBoardEValueType.@string;
                ret.stringVal = (string)v;
            }
            else if (t.IsEnum)
            {
                ret.BoardEValueType = EBoardEValueType.@int;
                ret.intVal = (int)v;
            }
            else if (t == typeof(object))
            {
                ret.BoardEValueType = EBoardEValueType.@object;
                ret.objectVal = v;
            }
            else
            {
                MyDebug.LogError($"Unexpected type {t}");
            }
            ret.objectVal = v;
            return ret;
        }
    }
}