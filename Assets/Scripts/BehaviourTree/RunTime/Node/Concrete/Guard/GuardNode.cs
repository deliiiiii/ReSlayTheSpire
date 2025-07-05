using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNode : NodeBase
    {
        [NonSerialized]
        protected Func<bool> Condition;

        public bool Judge()
        {
            bool ret = Condition();
            State.Value = ret ? EState.Succeeded : EState.Failed;
            return ret;
        }
    }
    
}