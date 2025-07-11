// #if UNITY_EDITOR
// using UnityEditor;
// #endif

using System;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeHolder : MonoBehaviour
    {
        public RootNode Root;
        public bool ShowStartTick = true;
        public bool IsRunning = false;

        void Update()
        {
            if (!IsRunning)
                return;
            Tick(Time.deltaTime);
        }

        public EState Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            return Root.Tick(dt);
        }
    }
}