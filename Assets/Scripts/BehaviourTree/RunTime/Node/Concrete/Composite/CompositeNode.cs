using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Multiple;
        protected LinkedListNode<NodeBase> curNode;
    }
}