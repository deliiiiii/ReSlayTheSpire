using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeDelay : ActionNode
    {
        [DrawnField]
        public float DelaySeconds;
        [ShowInInspector][ReadOnly]
        float timer;

        void OnEnable()
        {
            OnEnter = () => timer = 0;
            OnContinue = dt =>
            {
                timer += dt;
                isFinished = timer >= DelaySeconds;
            };
        }
        public override string ToString()
        {
            return $"Delay {DelaySeconds}s";
        }
    }
}