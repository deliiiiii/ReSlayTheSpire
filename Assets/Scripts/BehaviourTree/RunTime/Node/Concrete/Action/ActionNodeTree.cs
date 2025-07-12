using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeTree : ActionNode, IShowDetail
    {
        protected override void OnEnable()
        {
            base.OnEnable();
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
        [Required][CanBeNull]
        public RootNode SubTreeRoot;
        public new string GetDetail()
        {
            return $"设置延迟无效！\n{base.GetDetail()}SubTree:{SubTreeRoot?.name ?? "null"}";
        }
    }
}