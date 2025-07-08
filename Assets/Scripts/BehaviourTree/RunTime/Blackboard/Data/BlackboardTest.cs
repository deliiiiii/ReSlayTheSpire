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
        public int Intttt;
        public float Floattttt;
        public EState Stateeee;
        public bool Boolll;
        public string Stringggg;
        public Vector3 Vector3cc;
    }
}