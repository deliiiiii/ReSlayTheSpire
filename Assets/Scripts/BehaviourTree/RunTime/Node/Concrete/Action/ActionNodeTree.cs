using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeTree : ActionNode, IShowDetail
    {
        [Required][CanBeNull]
        public RootNode SubTreeRoot;

        protected override void OnEnableAfter()
        {
            OnContinue = dt =>
            {
                if (SubTreeRoot == null)
                {
                    IsFinished = true;
                    return;
                }
                var ret = SubTreeRoot.Tick(dt);
                if (ret == EState.Running)
                    return;
                IsFinished = true;
            };
        }
        public string GetDetail()
        {
            return $"SubTree[{SubTreeRoot?.name ?? "null"}]";
        }
    }
}