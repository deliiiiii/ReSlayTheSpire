using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNode : NodeBase
    {
        [NonSerialized]
        public Func<bool> Condition;
    }
    
}