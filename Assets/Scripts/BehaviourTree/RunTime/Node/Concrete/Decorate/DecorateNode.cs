using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public class DecorateNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
    }
    
    
}