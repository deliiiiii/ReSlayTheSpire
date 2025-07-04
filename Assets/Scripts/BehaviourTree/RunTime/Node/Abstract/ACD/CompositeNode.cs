using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : ACDNode
    {
        protected LinkedListNode<ACDNode> curNode;
        public override ACDNode AddChild(ACDNode child)
        {
            ChildList ??= new LinkedList<ACDNode>();
            ChildList.AddLast(child);
            return this;
        }
    }
}