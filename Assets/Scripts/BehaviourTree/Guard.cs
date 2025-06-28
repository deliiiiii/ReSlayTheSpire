using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class Guard
    {
        [ShowInInspector]
        public string Name => ToString();
        [HideInInspector]
        public Func<bool> Condition;
        
        public static Guard AlwaysTrue => new()
        {
            Condition = () => true
        };

        public override string ToString()
        {
            return "AlwaysTrue";
        }
    }
    public class GuardMoreThan : Guard
    {
        public int Value;
        public int Threshold;
        
        public GuardMoreThan(int value, int threshold)
        {
            Value = value;
            Threshold = threshold;
            Condition = () => Value > Threshold;
        }

        public override string ToString()
        {
            return $"{Value} > {Threshold}";
        }
    }
}