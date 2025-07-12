using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Multiple;
        [CanBeNull] protected LinkedListNode<NodeBase> curNode;
    }
}