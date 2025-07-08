using System;
using UnityEngine;

namespace BehaviourTree
{
    public class ExTest : MonoBehaviour
    {
        [SerializeField]
        BlackboardTest board;
        void Update()
        {
            MyDebug.Log(board.Get<BlackboardTest, float>("Float"));
        }
    }
}