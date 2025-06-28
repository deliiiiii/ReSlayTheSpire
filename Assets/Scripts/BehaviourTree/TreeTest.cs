using System;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : MonoBehaviour
    {
        public Tree Tree = new Tree();

        void Awake()
        {
            Application.targetFrameRate = 30;
            Tree.Create().SetName("RootNode")
                .AddChild(new ActionNodeDebug("Start")).SetChildName("StartNode")
                .AddChild(new ActionNodeDelay(2f)).SetChildName("DelayNode")
                .AddChild(new ActionNodeDebug("End")).SetChildName("EndNode");
        }

        void Update()
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            Tree.Tick(Time.deltaTime);
        }
    }
}