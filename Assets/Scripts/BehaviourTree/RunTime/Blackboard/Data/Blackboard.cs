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
    public interface IBlackboard<out T> where T : Blackboard
    {
        public TField IGet<TField>(string fieldName)
        {
            MyDebug.Log(GetType() + " " + typeof(T));
            var param = Expression.Parameter(GetType(), "x");
            var field = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda<Func<T, TField>>(field, param);
            return lambda.Compile()(this as T);
        }
    }
    [Serializable]
    public class Blackboard : ScriptableObject
    {
        public TField Get<TField>(string fieldName) => ((IBlackboard<Blackboard>)this).IGet<TField>(fieldName);
    }
}