using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace BehaviourTree
{
    public class Blackboard : ScriptableObject
    {
        readonly SerializableDictionary<IBoardKey, Union> dic = new ();
        readonly SerializableDictionary<string, IBoardKey> fieldNameToKeyDic = new ();


        void OnEnable()
        {
            GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ForEach(member =>
                {
                    switch (member)
                    {
                        case FieldInfo fieldInfo:
                        {
                            var key = Activator.CreateInstance(typeof(BoardKey<>).MakeGenericType(fieldInfo.FieldType), fieldInfo.Name) as IBoardKey;
                            fieldNameToKeyDic.Add(fieldInfo.Name, key);
                            dic.TryAdd(key, new Union());
                            break;
                        }
                        case PropertyInfo propertyInfo:
                        {
                            var key = Activator.CreateInstance(typeof(BoardKey<>).MakeGenericType(propertyInfo.PropertyType), propertyInfo.Name) as IBoardKey;
                            fieldNameToKeyDic.Add(propertyInfo.Name, key);
                            dic.TryAdd(key, new Union());
                            break;
                        }
                    }
                });
        }

        public bool Get<T>(string fieldName, out T fieldValue) where T : class
        {
            if (dic.TryGetValue(fieldNameToKeyDic[fieldName], out var unionValue))
            {
                fieldValue = unionValue.intVal as T;
            }
            fieldValue = default;
            return false;
        }

        public void Set<T>(string fieldName, T value) where T : class
        {
            
        }

        public bool Remove<T>(string fieldName, out T value) where T : class
        {
            value = default;
            return false;
        }

        // Nullable支持 -- 重载方法
        public void Set<T>(string fieldName, T? value) where T : struct
        {
            
        }

        public bool Get<T>(string fieldName, out T? value) where T : struct
        {
            value = default;
            return false;
        }

        public bool Remove<T>(string fieldName, out T? value) where T : struct
        {
            value = default;
            return false;
        }
    }
}