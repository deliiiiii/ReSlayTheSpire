using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeTree : ActionNode
    {
        public override void OnDeserializeEnd()
        {
            base.OnDeserializeEnd();
            OnContinue = dt =>
            {
                var ret = SubTreeRoot?.TickTemplate(dt) ?? EState.Succeeded;
                return ret;
            };
        }

        protected override void Reset()
        {
            base.Reset();
            SubTreeRoot?.RecursiveDo(Reset);
        }


        [Required][CanBeNull]
        public RootNode SubTreeRoot;
        public override string GetDetail()
        {
            string warn = HasDelay ? "在这个节点上设置延迟无效！\n" : string.Empty;
            return $"{warn}{base.GetDetail()}SubTree:{SubTreeRoot?.name ?? "null"}";
        }
    }
}