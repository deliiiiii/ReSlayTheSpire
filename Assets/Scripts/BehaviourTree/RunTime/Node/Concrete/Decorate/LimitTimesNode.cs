using System;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class LimitTimesNode : DecorateNode, IShowDetail
    {
        [SerializeReference]
        public NodeWithEvent NodeWithEvent;
        
        public int LimitTimes = 1;
        public int LimitTimer = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            BTEvent.RegisterEvent((NodeWithEvent.EventK1, NodeWithEvent.EventK2), OnReset);
        }

        void OnDisable()
        {
            BTEvent.UnRegisterEvent((NodeWithEvent.EventK1, NodeWithEvent.EventK2), OnReset);
        }

        protected override void OnReset()
        {
            base.OnReset();
            LimitTimer = 0;
        }

        protected override EState Tick(float dt)
        {
            if (LimitTimer >= LimitTimes)
            {
                LastChild?.RecursiveDo(CallReset);
                return EState.Succeeded;
            }
            var ret = LastChild?.TickTemplate(dt) ?? EState.Succeeded;
            if(ret == EState.Succeeded)
                LimitTimer++;
            ret = EState.Running;
            return ret;
        }
        public string GetDetail()
        {
            StringBuilder sb =  new StringBuilder();
            sb.Append($"Limited:{LimitTimer}/{LimitTimes}");
            if (NodeWithEvent != null)
            {
                sb.Append("\n");
                sb.Append($"【Reset】OnEvent {NodeWithEvent.EventK1}::{NodeWithEvent.EventK2}");
            }
            return sb.ToString();
        }
        
    }
}