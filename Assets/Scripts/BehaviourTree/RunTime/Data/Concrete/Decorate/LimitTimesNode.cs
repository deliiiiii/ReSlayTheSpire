using System;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class LimitTimesNode : DecorateNode
    {
        [SerializeReference]
        public NodeWithEvent NodeWithEvent;
        
        public int LimitTimes = 1;
        public int LimitTimer = 0;

        public override void OnDeserializeEnd()
        {
            base.OnDeserializeEnd();
            BTEvent.RegisterEvent((NodeWithEvent.EventK1, NodeWithEvent.EventK2), Reset);
        }

        void OnDisable()
        {
            BTEvent.UnRegisterEvent((NodeWithEvent.EventK1, NodeWithEvent.EventK2), Reset);
        }

        protected override void Reset()
        {
            base.Reset();
            LimitTimer = 0;
        }

        protected override EState Tick(float dt)
        {
            if (LimitTimer >= LimitTimes)
            {
                LastChild?.RecursiveDo(Reset);
                return EState.Succeeded;
            }
            var ret = LastChild?.TickTemplate(dt) ?? EState.Succeeded;
            if(ret == EState.Succeeded)
                LimitTimer++;
            ret = EState.Running;
            return ret;
        }
        public override string GetDetail()
        {
            StringBuilder sb =  new StringBuilder();
            sb.Append($"Limited:{LimitTimer}/{LimitTimes}");
            if (NodeWithEvent != null)
            {
                sb.Append("\n");
                sb.Append($"【Reset OnReceiveEvent】{NodeWithEvent.EventK1}::{NodeWithEvent.EventK2}");
            }
            return sb.ToString();
        }
        
    }
}