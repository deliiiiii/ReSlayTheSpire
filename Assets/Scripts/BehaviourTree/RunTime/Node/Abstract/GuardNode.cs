using System;

namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [NonSerialized]
        public Func<bool> Condition;
    }
    
}