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
            MyDebug.Log($"DelayNode OnEnable {DelaySeconds}s", LogType.Tick);
            OnEnter = () => Timer = 0;
        }

        protected override async Task<EState> OnContinueAsync()
        {
            await Task.Delay((int)(DelaySeconds * 1000));
            return EState.Succeeded;
        }

        public override string ToString()
        {
            return $"Delay {DelaySeconds}s";
        }
    }
}