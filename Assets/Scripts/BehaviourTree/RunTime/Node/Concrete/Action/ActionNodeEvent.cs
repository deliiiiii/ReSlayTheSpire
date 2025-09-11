using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeEvent : ActionNode
    {
        [SerializeReference]
        public NodeWithEvent NodeWithEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            OnDelayEnd += () => BTEvent.SendEvent((NodeWithEvent.EventK1, NodeWithEvent.EventK2));
        }
        public override string GetDetail()
        {
            var sb = new StringBuilder();
            sb.Append(base.GetDetail());
            if (sb.Length > 0)
                sb.Append("\n");
            sb.Append("【SendEvent】");
            sb.Append(NodeWithEvent == null 
                ? string.Empty 
                : $"{NodeWithEvent.EventK1.ToString()}::{NodeWithEvent.EventK2}");
            return sb.ToString();
        }
    }
}