using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [ShowInInspector]
        public string Name => ToString();
        // [HideInInspector]
        public Func<bool> Condition;
    }
}