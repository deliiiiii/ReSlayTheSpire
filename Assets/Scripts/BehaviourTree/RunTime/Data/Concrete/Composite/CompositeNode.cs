using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : BTNodeData
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Multiple;
        [CanBeNull] protected LinkedListNode<BTNodeData> curNode;

        protected override void Reset()
        {
            base.Reset();
            curNode = null;
        }
    }
}