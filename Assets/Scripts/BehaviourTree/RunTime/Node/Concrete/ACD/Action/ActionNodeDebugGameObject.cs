using System;
using UnityEngine;

namespace BehaviourTree
{
    public class ActionNodeDebugGameObject : ActionNode
    {
        public GameObject Go;

        void OnEnable()
        {
            OnContinue = _ =>
            {
                MyDebug.Log($" pos {Go.transform.localPosition}");
                isFinished = true;
            };
        }
    }
}