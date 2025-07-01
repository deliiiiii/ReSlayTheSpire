using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public abstract class ActionNodeEditor<T> : NodeBaseEditor<T> where T : ActionNode
    {
        protected override T CreateConcreteNode(Type concreteT)
        {
            if(concreteT == typeof(ActionNodeSet<>))
                return Activator.CreateInstance(typeof(ActionNodeSet<>).MakeGenericType(typeof(int))) as T;
            return base.CreateConcreteNode(concreteT);
        }
    }
}