using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNodeMoreThan : GuardNode
    {
        [DrawnField]
        public int Value;
        [DrawnField]
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