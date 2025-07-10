using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeDelay : ActionNode
    {
        public float DelaySeconds;
        [ReadOnly]
        public float Timer;

        void OnEnable()
        {
            OnEnter = () => Timer = 0;
            OnContinue = dt =>
            {
                Timer += dt;
                IsFinished = Timer >= DelaySeconds;
            };
        }
        public override string ToString()
        {
            return $"Delay {DelaySeconds}s";
        }
    }
}