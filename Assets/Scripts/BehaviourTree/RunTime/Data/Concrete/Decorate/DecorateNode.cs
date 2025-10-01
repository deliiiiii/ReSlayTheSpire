using System;

namespace BehaviourTree
{
    [Serializable]
    public class DecorateNode : BTNodeData
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
    }
    
    
}