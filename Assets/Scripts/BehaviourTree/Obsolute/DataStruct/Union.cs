// using System;
// using System.Runtime.InteropServices;
// using JetBrains.Annotations;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace BehaviourTree
// {
//     [Serializable]
//     public enum EBoardEValueType : byte
//     {
//         Undefined = 0, // 表示Key不存在
//         @null = 1, // 表示Key存在，但值为null
//         @int = 2,
//         @long = 3,
//         @float = 4,
//         @double = 5,
//         @bool = 6,
//         @Vector3 = 7,
//         // ...
//         @object = 15, // 引用类型
//     }
//     
//     [Serializable]
//     [StructLayout(LayoutKind.Explicit)]
//     public struct Union : IEquatable<Union>, IComparable<Union>
//     {
//         [FieldOffset(0)] 
//         public EBoardEValueType BoardEValueType;
//         [FieldOffset(1)] [ShowIf(nameof(isInt))]
//         public int intVal; bool isInt => BoardEValueType == EBoardEValueType.@int;
//         [FieldOffset(1)] [ShowIf(nameof(isLong))]
//         public long longVal; bool isLong => BoardEValueType == EBoardEValueType.@long;
//         [FieldOffset(1)] [ShowIf(nameof(isFloat))]
//         public float floatVal; bool isFloat => BoardEValueType == EBoardEValueType.@float;  
//         [FieldOffset(1)] [ShowIf(nameof(isDouble))]
//         public double doubleVal; bool isDouble => BoardEValueType == EBoardEValueType.@double;
//         [FieldOffset(1)] [ShowIf(nameof(isBool))]
//         public bool boolVal; bool isBool => BoardEValueType == EBoardEValueType.@bool;
//         [FieldOffset(1)] [ShowIf(nameof(isVector3))] // sizeof(Vector3) = 12
//         public Vector3 vector3; bool isVector3 => BoardEValueType == EBoardEValueType.@Vector3;  
//         // ... 内存对齐后需要偏移16
//         [FieldOffset(16)] [CanBeNull] [ShowIf(nameof(isObject))]
//         public object objectVal; bool isObject => BoardEValueType == EBoardEValueType.@object;
//
//
//         public bool Equals(Union other)
//         {
//             if (BoardEValueType != other.BoardEValueType)
//                 return false;
//
//             return BoardEValueType switch
//             {
//                 EBoardEValueType.@int      => intVal == other.intVal,
//                 EBoardEValueType.@long     => longVal == other.longVal,
//                 EBoardEValueType.@float    => Math.Abs(floatVal - other.floatVal) < 0.0001f,
//                 EBoardEValueType.@double   => Math.Abs(doubleVal - other.doubleVal) < 0.0001,
//                 EBoardEValueType.@bool     => boolVal == other.boolVal,
//                 EBoardEValueType.@Vector3  => vector3.Equals(other.vector3),
//                 EBoardEValueType.@object   => ReferenceEquals(objectVal, other.objectVal),
//                 _                          => true
//             };
//         }
//         
//         public int CompareTo(Union other)
//         {
//             if (BoardEValueType != other.BoardEValueType)
//                 return 0;
//             
//             return BoardEValueType switch
//             {
//                 EBoardEValueType.@int      => intVal.CompareTo(other.intVal),
//                 EBoardEValueType.@long     => longVal.CompareTo(other.longVal),
//                 EBoardEValueType.@float    => floatVal.CompareTo(other.floatVal),
//                 EBoardEValueType.@double   => doubleVal.CompareTo(other.doubleVal),
//                 EBoardEValueType.@bool     => boolVal.CompareTo(other.boolVal),
//                 EBoardEValueType.@Vector3  => vector3.magnitude.CompareTo(other.vector3.magnitude),
//                 EBoardEValueType.@object   => 0,
//                 _                          => 0,
//             };
//         }
//
//         public static Union Null = Create(null, null);
//         public static Union Create(Type t, object v)
//         {
//             var ret = new Union();
//             if (t == typeof(int))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@int;
//                 ret.intVal = (int)v;
//             }
//             else if (t == typeof(long))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@long;
//                 ret.longVal = (long)v;
//             }
//             else if (t == typeof(float))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@float;
//                 ret.floatVal = (float)v;
//             }
//             else if (t == typeof(double))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@double;
//                 ret.doubleVal = (double)v;
//             }
//             else if (t == typeof(bool))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@bool;
//                 ret.boolVal = (bool)v;
//             }
//             else if (t == typeof(Vector3))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@Vector3;
//                 ret.vector3 = (Vector3)v;
//             }
//             else if (t == typeof(object))
//             {
//                 ret.BoardEValueType = EBoardEValueType.@object;
//                 ret.objectVal = v;
//             }
//             else
//             {
//                 if(t != null && v != null)
//                     MyDebug.LogError($"Unexpected type {t}");
//                 ret.BoardEValueType = EBoardEValueType.Undefined;
//             }
//             return ret;
//         }
//     }
// }