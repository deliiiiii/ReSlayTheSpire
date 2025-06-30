using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : NodeBase
    {
        // [ShowInInspector]
        protected LinkedList<NodeBase> childLinkedList;
        protected LinkedListNode<NodeBase> curNode;
        
        public override NodeBase AddChild(NodeBase child)
        {
            childLinkedList ??= new LinkedList<NodeBase>();
            ChildList ??= new List<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childLinkedList.AddLast(child);
            ChildList.Add(child);
            return this;
        }
        
        // public T AddChildTo<T>(T child) where T : NodeBase
        // {
        //     childLinkedList ??= new LinkedList<NodeBase>();
        //     child.Parent = this;
        //     child.Tree = Tree;
        //     childLinkedList.AddLast(child);
        //     return child;
        // }
        
        public CompositeNode RemoveChild(NodeBase child)
        {
            if (childLinkedList == null || !childLinkedList.Contains(child))
            {
                MyDebug.LogError($"Child \"{child.NodeName}\" not found in {NodeName} children list.");
                return this;
            }
            childLinkedList.Remove(child);
            ChildList.Remove(child);
            child.Parent = null;
            child.Tree = null;
            return this;
        }

        public CompositeNode ClearChildren()
        {
            childLinkedList?.Clear();
            ChildList?.Clear();
            return this;
        }
    }
}