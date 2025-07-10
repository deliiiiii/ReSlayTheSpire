using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeDelay : ActionNode, IShowDetail
    {
        public float DelaySeconds;
        [ReadOnly]
        public float Timer;

        protected override void OnEnableAfter()
        {
            OnEnter = () => Timer = 0;
            OnContinue = dt =>
            {
                Timer += dt;
                IsFinished = Timer >= DelaySeconds;
            };
        }

        public string GetDetail()
        {
            string t = Timer switch
            {
                < 1 => $"{Timer * 1000:F0}",
                < 60 => $"{Timer:F2}",
                _ => $"{Timer / 60:F2}",
            };
            string tTanni = (Timer, DelaySeconds) switch
            {
                (<= 1,<= 1) or
                (>1 and <=60,>1 and <=60) or
                (>60 and <=3600,>60 and <=3600) => "",
                (<=1, _) => "ms",
                (<=60, _) => "s",
                (_, _) => "min",
            };
            string dWithTanni = DelaySeconds switch
            {
                < 1 => $"{DelaySeconds * 1000:F0}ms",
                < 60 => $"{DelaySeconds:F2}s",
                _ => $"{DelaySeconds / 60:F2}min",
            };
            return $"{t}{tTanni}/{dWithTanni}";
        }
    }
}