using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class ExTest : MonoBehaviour
    {
        public Blackboard Board;
        void Update()
        {
            MyDebug.Log(Board.Get<float>("Float"));
        }
    }
}