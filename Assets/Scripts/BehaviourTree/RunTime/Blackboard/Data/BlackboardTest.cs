using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(BlackboardTest), menuName = "BehaviourTree/" + nameof(BlackboardTest))]
    public class BlackboardTest : Blackboard, IBlackboard<BlackboardTest>
    {
        int i;
        public float Float;
        // string s;
        // EState eState;
        public int I 
        {
            get => i;
            set => i = value;
        }
    }
}