using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : ACDNode
    {
        protected LinkedListNode<ACDNode> curNode;
        
        public override EState OnTickChild(float dt)
        {
            throw new NotImplementedException();
        }

        public override void OnFail()
        {
            var fNode = ChildLinkedList?.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }

        public override ACDNode AddChild(ACDNode child)
        {
            ChildList ??= new List<ACDNode>();
            ChildList.Add(child);
            return this;
        }
    }
}