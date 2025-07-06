using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNodeMoreThan : GuardNode
    {
        public int Value;
        public int Threshold;

        void OnEnable()
        {
            Condition = () => Value > Threshold;
        }
        public override string ToString()
        {
            return $"{Value} > {Threshold}";
        }
    }
}