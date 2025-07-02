using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : ACDNode
    {
        protected LinkedListNode<ACDNode> curNode;
        public override ACDNode AddChild(ACDNode child)
        {
            ChildList ??= new LinkedList<ACDNode>();
            child.Parent = this;
            // child.Tree = Tree;
            ChildList.AddLast(child);
            return this;
        }
    }
}