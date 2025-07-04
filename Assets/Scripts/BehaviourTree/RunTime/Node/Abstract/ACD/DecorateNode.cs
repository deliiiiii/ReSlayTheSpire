using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public abstract class DecorateNode : ACDNode
    {
        public override ACDNode AddChild(ACDNode child)
        {
            if (FirstChild != null)
            {
                MyDebug.LogError("InverseNode can only have one child.");
                return this;
            }
            ChildList ??= new LinkedList<ACDNode>();
            ChildList.AddLast(child);
            return this;
        }
    }
    
    
}