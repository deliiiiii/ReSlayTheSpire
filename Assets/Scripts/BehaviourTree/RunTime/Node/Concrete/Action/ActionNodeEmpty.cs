using System;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeEmpty : ActionNode, IShowDetail
    {
        public new string GetDetail()
        {
            return base.GetDetail();
        }
    }
}