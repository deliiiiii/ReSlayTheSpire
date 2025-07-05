using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : ACDNode
    {
        protected LinkedListNode<ACDNode> curNode;
        public override EState OnTick(float dt)
        {
            return EState.Succeeded;
        }

        public override void OnFail()
        {
            curNode = null;
        }

        public override ACDNode AddChild(ACDNode child)
        {
            ChildList ??= new List<ACDNode>();
            ChildList.Add(child);
            return this;
        }
    }
}