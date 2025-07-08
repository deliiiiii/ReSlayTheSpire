using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(BlackboardTest), menuName = "BehaviourTree/" + nameof(BlackboardTest))]
    public class BlackboardTest : Blackboard
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
        //
        // public List<int> IntList;
        // public Type T;
        // public List<EState> EList;

    }
}