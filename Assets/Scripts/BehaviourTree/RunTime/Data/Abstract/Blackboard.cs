using System;
using System.Linq.Expressions;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class Blackboard : ScriptableObject
    {
        public object Get(string fieldName)
        {
            var param = Expression.Parameter(GetType(), "x");
            var field = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda(field, param);
            return lambda.Compile().DynamicInvoke(this);
        }
    }
}