using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : NodeBase
    {
        [ShowInInspector]
        protected LinkedList<NodeBase> childList;
        [ShowInInspector]
        protected LinkedListNode<NodeBase> curNode;
        
        public CompositeNode SetName(string name)
        {
            Name = name;
            return this;
        }

        public CompositeNode SetChildName(string name)
        {
            childList.Last.Value.Name = name;
            return this;
        }
        
        public CompositeNode AddChild(NodeBase child)
        {
            childList ??= new LinkedList<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childList.AddLast(child);
            return this;
        }
        public CompositeNode RemoveChild(NodeBase child)
        {
            if (childList == null || !childList.Contains(child))
            {
                MyDebug.LogError($"Child \"{child.Name}\" not found in {Name} children list.");
                return this;
            }
            childList.Remove(child);
            child.Parent = null;
            child.Tree = null;
            return this;
        }

        public CompositeNode ClearChildren()
        {
            childList?.Clear();
            return this;
        }
    }

    [Serializable]
    public class SequenceNode : CompositeNode
    {
        public override bool OnTick(float dt)
        {
            // curNode ??= childList.First;
            curNode = childList.First;
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (!res)
                    return false;
                curNode = curNode.Next;
            }
            return true;
            
        }
    }
    
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        public override bool OnTick(float dt)
        {
            // curNode ??= childList.First;
            curNode = childList.First;
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res)
                    return true;
                curNode = curNode.Next;
            }
            return false;
        }
    }
}