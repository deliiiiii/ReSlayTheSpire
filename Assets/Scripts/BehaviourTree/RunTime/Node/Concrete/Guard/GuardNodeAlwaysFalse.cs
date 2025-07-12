using System;

namespace BehaviourTree
{
    public class GuardNodeAlwaysFalse: GuardNode
    {
        protected override Func<bool> Condition => () => false;
    }
}