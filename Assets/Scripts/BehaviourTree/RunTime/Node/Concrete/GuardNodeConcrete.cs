using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNodeAlwaysTrue : GuardNode
    {
        public GuardNodeAlwaysTrue()
        {
            Condition = () => true;
        }
        public override string ToString()
        {
            return "Always True";
        }
    }
    
    [Serializable]
    public class GuardNodeMoreThan : GuardNode
    {
        [DrawnField]
        public int Value;
        [DrawnField]
        public int Threshold;

        public GuardNodeMoreThan()
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