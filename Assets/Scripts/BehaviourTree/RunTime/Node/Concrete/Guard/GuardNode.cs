using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        [NonSerialized]
        protected Func<bool> Condition;

        public bool Judge()
        {
            var ret = Condition();
            State.Value = ret ? EState.Succeeded : EState.Failed;
            return ret;
        }
    }
    
}