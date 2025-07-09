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
        public int Intttt;
        public float Floatttt;
        public EState Stateeee;
        public bool Boollll;
        public string Stringggg;
        public Vector3 Vector3333;
    }
}