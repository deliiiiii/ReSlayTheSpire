using System;
using System.Reflection;
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
        @object = 15, // 引用类型,不支持序列化。。。
    }
    
    [Serializable]
    public struct Union : IComparable<Union>
    {
        [ReadOnly]
        public EBoardEValueType BoardEValueType;
        [SerializeField][ShowIf(nameof(isInt))]
        int intVal; bool isInt => BoardEValueType == EBoardEValueType.@int;
        [SerializeField][ShowIf(nameof(isLong))]
        long longVal; bool isLong => BoardEValueType == EBoardEValueType.@long;
        [SerializeField][ShowIf(nameof(isFloat))]
        float floatVal; bool isFloat => BoardEValueType == EBoardEValueType.@float;  
        [SerializeField][ShowIf(nameof(isDouble))]
        double doubleVal; bool isDouble => BoardEValueType == EBoardEValueType.@double;
        [SerializeField][ShowIf(nameof(isBool))]
        bool boolVal; bool isBool => BoardEValueType == EBoardEValueType.@bool;
        [SerializeField][ShowIf(nameof(isString))]
        string stringVal; bool isString => BoardEValueType == EBoardEValueType.@string;
        [SerializeField][ShowIf(nameof(isVector3))]
        Vector3 vector3; bool isVector3 => BoardEValueType == EBoardEValueType.@Vector3;  

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
            // Observable<T>
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Observable<>))
            {
                var innerType = t.GetGenericArguments()[0];
                ret.BoardEValueType = ConvertType(innerType);
                if (ret.BoardEValueType == EBoardEValueType.@object)
                {
                    MyDebug.LogError($"Observable<{innerType.Name}> is not supported in Union");
                }
                else
                {
                    ret = Create(innerType, typeof(Observable<>)
                        .MakeGenericType(innerType)
                        .GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(v));
                }
                
            }
            else
            {
                MyDebug.LogError($"Unexpected type {t}");
            }
            return ret;
        }
        public static EBoardEValueType ConvertType(Type t)
        {
            if (t == typeof(int)) return EBoardEValueType.@int;
            if (t == typeof(long)) return EBoardEValueType.@long;
            if (t == typeof(float)) return EBoardEValueType.@float;
            if (t == typeof(double)) return EBoardEValueType.@double;
            if (t == typeof(bool)) return EBoardEValueType.@bool;
            if (t == typeof(string)) return EBoardEValueType.@string;
            if (t == typeof(Vector3)) return EBoardEValueType.@Vector3;
            if (t.IsEnum) return EBoardEValueType.@int;
            if (t.GetGenericTypeDefinition() == typeof(Observable<>))
                return ConvertType(t.GetGenericArguments()[0]);
            return EBoardEValueType.@object;
        }
        public object GetValue()
        {
            if(isInt)
                return intVal;
            if (isLong)
                return longVal;
            if (isFloat)
                return floatVal;
            if (isDouble)
                return doubleVal;
            if (isBool)
                return boolVal;
            if (isString)
                return stringVal;
            if (isVector3)
                return vector3;
            return 0;
        }
    }
}