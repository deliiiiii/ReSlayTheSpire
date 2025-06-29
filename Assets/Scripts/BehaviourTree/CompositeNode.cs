using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{

    public static class CompositeNodeExtensions
    {
        public static T ToChild<T>(this CompositeNode node) where T : NodeBase
        {
            return node.childList?.Last?.Value as T;
        }
    }
    [Serializable]
    public abstract class CompositeNode : NodeBase
    {
        [ShowInInspector]
        public LinkedList<NodeBase> childList;
        protected LinkedListNode<NodeBase> curNode;
        [ShowInInspector]
        NodeBase curNodeTrue => curNode?.Value;

        public CompositeNode SetChildName(string name)
        {
            childList.Last.Value.Name = name;
            return this;
        }
        
        // public NodeBase ToChild()
        // {
        //     return childList?.Last.Value;
        // }
        
        public CompositeNode AddChildStay(NodeBase child)
        {
            childList ??= new LinkedList<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childList.AddLast(child);
            return this;
        }
        
        public T AddChildTo<T>(T child) where T : NodeBase
        {
            childList ??= new LinkedList<NodeBase>();
            child.Parent = this;
            child.Tree = Tree;
            childList.AddLast(child);
            return child;
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
        public override void OnFail()
        {
            var fNode = childList.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
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
        public override void OnFail()
        {
            var fNode = childList.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }
    }
}