using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class CompositeNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Multiple;
        [SerializeField]
        protected int curNodeId;
    }
}