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
        // public GuardBaseMoreThan(int value, int threshold)
        // {
        //     this.value = value;
        //     this.threshold = threshold;
        //     Condition = () => this.value > this.threshold;
        // }

        public override string ToString()
        {
            return $"{Value} > {Threshold}";
        }
    }
}