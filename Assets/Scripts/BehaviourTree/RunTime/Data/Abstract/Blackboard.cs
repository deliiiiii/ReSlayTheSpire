using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class Blackboard : ScriptableObject
    {
        public object Get(string fieldName)
        {
            // var param = Expression.Parameter(GetType(), "x");
            // var field = Expression.Field(param, fieldName);
            // var lambda = Expression.Lambda(field, param);
            // return lambda.Compile().DynamicInvoke(this);
            if (!fieldInfoCache.TryGetValue(fieldName, out var field))
            {
                fieldInfoCache.Add(fieldName, GetType().GetField(fieldName));
            }
            return field?.GetValue(this);
        }

        Dictionary<string, FieldInfo> fieldInfoCache = new();
        public void Set(string fieldName, object value)
        {
            if (!fieldInfoCache.TryGetValue(fieldName, out var fieldInfo))
            {
                fieldInfoCache.Add(fieldName, GetType().GetField(fieldName));
            }

            if (fieldInfo?.FieldType.IsGenericType ?? false)
            {
                var genericType = fieldInfo?.FieldType.GetGenericTypeDefinition();
                var tType = fieldInfo?.FieldType.GetGenericArguments()[0];
                if (genericType == typeof(Observable<>))
                {
                    genericType.MakeGenericType(tType)
                        .GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.SetValue(fieldInfo.GetValue(this), value);
                }
                return;
            }
            fieldInfo?.SetValue(this, value);
        }
    }
}