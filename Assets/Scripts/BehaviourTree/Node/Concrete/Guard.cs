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
        // [HideInInspector]
        public Func<bool> Condition;
        public static Guard AlwaysTrue = new GuardAlwaysTrue();
    }
    public class GuardAlwaysTrue : Guard
    {
        public GuardAlwaysTrue()
        {
            Condition = () => true;
        }
        public override string ToString()
        {
            return "Always True";
        }
    }
    
    public class GuardMoreThan : Guard
    {
        readonly int value;
        readonly int threshold;
        
        public GuardMoreThan(int value, int threshold)
        {
            this.value = value;
            this.threshold = threshold;
            Condition = () => this.value > this.threshold;
        }

        public override string ToString()
        {
            return $"{value} > {threshold}";
        }
    }
}