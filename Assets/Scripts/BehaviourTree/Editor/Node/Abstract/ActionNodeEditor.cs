using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public abstract class ActionNodeEditor : NodeBaseEditor<ActionNode>
    {
        protected override ActionNode CreateNodeBase(Type concreteT)
        {
            //如果concreteT是ActionNodeSet<int> or ActionNodeSet<bool>
            // if(concreteT.IsGenericType && concreteT.GetGenericTypeDefinition() == typeof(ActionNodeSet<>))
            if(concreteT.IsGenericType)
                return Activator.CreateInstance(concreteT.MakeGenericType(typeof(int))) as T;
            return base.CreateNodeBase(concreteT);
        }
    }
}