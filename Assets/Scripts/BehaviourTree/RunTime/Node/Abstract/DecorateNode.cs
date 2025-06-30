using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public abstract class DecorateNode : NodeBase
    {
        public override NodeBase AddChild(NodeBase child)
        {
            if (ToChild() != null)
            {
                MyDebug.LogError("InverseNode can only have one child.");
                return this;
            }
            childList ??= new LinkedList<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childList.AddLast(child);
            return this;
        }
    }
    
    
}