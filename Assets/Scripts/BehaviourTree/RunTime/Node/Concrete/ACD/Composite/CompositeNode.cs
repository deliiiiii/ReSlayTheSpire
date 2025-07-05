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
            throw new NotImplementedException();
        }

        public override void OnFail()
        {
            throw new NotImplementedException();
        }

        public override ACDNode AddChild(ACDNode child)
        {
            ChildList ??= new List<ACDNode>();
            ChildList.Add(child);
            return this;
        }
    }
}