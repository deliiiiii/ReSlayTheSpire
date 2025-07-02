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
            child.Parent = this;
            child.Tree = Tree;
            ChildList.AddLast(child);
            return this;
        }
    }
    
    
}