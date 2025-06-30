using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : NodeBase
    {
        protected LinkedListNode<NodeBase> curNode;
        public override NodeBase AddChild(NodeBase child)
        {
            childList ??= new LinkedList<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childList.AddLast(child);
            return this;
        }
    }
}