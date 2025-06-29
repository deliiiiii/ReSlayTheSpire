using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public abstract class CompositeNode : NodeBase
    {
        [ShowInInspector]
        public LinkedList<NodeBase> childList;
        protected LinkedListNode<NodeBase> curNode;
        
        public override NodeBase GetChild()
        {
            return childList?.Last?.Value;
        }

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
        public override EState OnTick(float dt)
        {
            if (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res == EState.Succeeded)
                {
                    curNode = curNode.Next;
                }
            }
            else
            {
                curNode = childList.First;
            }
            
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running)
                {
                    return res;
                }
                if (res is EState.Failed)
                {
                    curNode = null;
                    return res;
                }
                curNode = curNode.Next;
            }
            return EState.Succeeded;
            
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
        public override EState OnTick(float dt)
        {
            // curNode ??= childList.First;
            curNode = childList.First;
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running or EState.Succeeded)
                    return res;
                curNode = curNode.Next;
            }
            return EState.Failed;
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