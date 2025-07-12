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
                var ret = SubTreeRoot?.TickTemplate(dt) ?? EState.Succeeded;
                return ret;
            };
        }

        protected override void OnReset()
        {
            base.OnReset();
            SubTreeRoot?.RecursiveDo(CallReset);
        }


        [Required][CanBeNull]
        public RootNode SubTreeRoot;
        public new string GetDetail()
        {
            string warn = HasDelay ? "在这个节点上设置延迟无效！\n" : string.Empty;
            return $"{warn}{base.GetDetail()}SubTree:{SubTreeRoot?.name ?? "null"}";
        }
    }
}