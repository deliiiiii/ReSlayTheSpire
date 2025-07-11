using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeUnityEvent : ActionNode, IShowDetail
    {
        [SerializeField]
        public UnityEvent Event;

        public ActionNodeUnityEvent()
        {
            OnEnter = () =>
            {
                Event?.Invoke();
            };
        }
        

        public string GetDetail()
        {
            var sb = new StringBuilder();
            int c = Event?.GetPersistentEventCount() ?? 0;
            for(int i = 0; i < c; i++)
            {
                var methodName = Event.GetPersistentMethodName(i);
                var target = Event.GetPersistentTarget(i);
                sb.AppendLine($"{methodName} on {target}");
                sb.Replace("UnityEngine.", string.Empty);
            }
            return sb.ToString() == string.Empty ? "No event." : sb.ToString();
        }
    }
}