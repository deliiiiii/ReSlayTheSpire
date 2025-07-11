using System;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeTreeHolder : ActionNode
    {
        public TreeHolder TreeHolder;
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        public ActionNodeTreeHolder()
        {
            OnContinue = dt =>
            {
                if (TreeHolder == null)
                {
                    IsFinished = true;
                    return;
                }
                var ret = TreeHolder.Tick(dt);
                if (ret == EState.Running)
                    return;
                IsFinished = true;
            };
        }
    }
}