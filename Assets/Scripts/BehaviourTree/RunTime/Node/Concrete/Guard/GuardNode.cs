using System;
using JetBrains.Annotations;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        [NotNull] protected virtual Func<bool> Condition { get; } = () => true;

        public bool Judge()
        {
            var ret = Condition() && (GuardNode?.Judge() ?? true);
            State.Value = ret ? EState.Succeeded : EState.Failed;
            return ret;
        }
    }
    
}