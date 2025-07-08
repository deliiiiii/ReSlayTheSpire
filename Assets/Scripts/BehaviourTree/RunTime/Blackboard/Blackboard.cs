using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using General;
using Sirenix.Utilities;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class Blackboard : ScriptableObject
    {
        // SerializableDictionary<Type, Func<string, Func<Blackboard, IObservable<object>>>> exDic = new ();

        public SerializableDictionary<Type, Func<Blackboard, string, object>> exxDic = new();
        public Wrap<int> WInt;
        public static Func<Blackboard, TField> CreateFieldGetter<TField>(Blackboard board, string fieldName) where TField : IComparable<TField>
        {
            var param = Expression.Parameter(board.GetType(), "x");
            var field = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda<Func<Blackboard, TField>>(field, param);
            return lambda.Compile();
        }
        
        // public static Func<Blackboard, TField> CreateFieldGetter<TField>(Blackboard board, string fieldName) where TField : IComparable
        // {
        //     var param = Expression.Parameter(board.GetType(), "x");
        //     var field = Expression.Field(param, fieldName);
        //     var lambda = Expression.Lambda<Func<Blackboard, TField>>(field, param);
        //     return lambda.Compile();
        // }
        //
        // static Func<T, TProperty> CreatePropertyGetter<T, TProperty>(string propertyName) where T : Blackboard where TProperty : IComparable<TProperty>
        // {
        //     var param = Expression.Parameter(typeof(T), "x");
        //     var property = Expression.Property(param, propertyName);
        //     var lambda = Expression.Lambda<Func<T, TProperty>>(property, param);
        //     return lambda.Compile();
        // }
        
        // public static Func<Blackboard, T> CreateFieldGetterForTBoard<T>(string fieldName) where T : IComparable<T>
        // {
        //     return CreateFieldGetter<Blackboard, T>(this, fieldName);
        // }
        
        void OnEnable()
        {
            GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ForEach(fieldInfo =>
                {
                    var fieldType = fieldInfo.FieldType;
                    if (fieldType == typeof(float))
                    {
                        var lambda = GetType().GetMethod(nameof(CreateFieldGetter))!
                            .MakeGenericMethod(fieldInfo.GetType())
                            .Invoke(this, new object[] { fieldInfo.Name });
                        // 等同于
                        CreateFieldGetter<float>(this, fieldInfo.Name);
                    }
                    if (fieldType == typeof(float))
                    {
                        exxDic.TryAdd(fieldType, CreateFieldGetter<float>);
                    }
                    else if (fieldType == typeof(int))
                    {
                        exxDic.TryAdd(fieldType, CreateFieldGetter<int>);
                    }
                    else if (fieldType == typeof(double))
                    {
                        exxDic.TryAdd(fieldType, CreateFieldGetter<double>);
                    }
                    else if (fieldType == typeof(string))
                    {
                        exxDic.TryAdd(fieldType, CreateFieldGetter<string>);
                    }
                    // else if (fieldType == typeof(Enum))
                    // {
                    //     exxDic.TryAdd(fieldType, CreateFieldGetter<Enum>);
                    // }
                });
        }

        public T Get<T>(string fieldName)
        {
            return (T)exxDic[typeof(T)](this, fieldName);
        }
        
    }
}