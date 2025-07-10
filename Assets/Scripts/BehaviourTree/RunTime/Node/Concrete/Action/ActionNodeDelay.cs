using System;
using System.Threading.Tasks;
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
            OnContinueAsync = Action();
        }

        async Task Action()
        {
            await Task.Delay((int)(DelaySeconds * 1000));
        }

        public override string ToString()
        {
            return $"Delay {DelaySeconds}s";
        }
    }
}