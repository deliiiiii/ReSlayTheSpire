using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeEvent : ActionNode, IShowDetail
    {
        public EEvent EventType;
        protected override void OnEnableAfter()
        {
            OnEnter = () => { BTEvent.SendEvent(EventType); };
        }
        
        public string GetDetail()
        {
            return $"Sent {EventType.ToString()}";
        }
    }
}