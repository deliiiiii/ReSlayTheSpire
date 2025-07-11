using System;

namespace BehaviourTree
{
    [Serializable]
    public class DecorateNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
    }
    
    
}