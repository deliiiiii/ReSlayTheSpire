using System;
using UnityEngine;

namespace BehaviourTree
{
    public class ActionNodeDebugGameObject : ActionNode
    {
        public GameObject Go;

        void OnEnable()
        {
            OnEnter = () => MyDebug.Log($" pos {Go.transform.localPosition}");
        }
    }
}