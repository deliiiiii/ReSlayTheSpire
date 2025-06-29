using System;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : MonoBehaviour
    {
        public Tree Tree = new Tree();
        public bool TestBool = true;
        void Awake()
        {
            Application.targetFrameRate = 30;
            Tree.Create<SelectorNode>().SetName("RootNode")
                
                .AddChildStay(new SequenceNode()).SetName("Se1")
                .SetGuard(() => TestBool)
                .ToChild<SequenceNode>()
                    .AddChildStay(new ActionNodeDebug("Start")).SetChildName("StartNode")
                    .AddChildStay(new ActionNodeDelay(2f)).SetChildName("DelayNode")
                    .AddChildStay(new ActionNodeSet<bool>(false, tar => TestBool = tar)).SetChildName("Setter")
                    .AddChildStay(new ActionNodeDebug("End")).SetChildName("EndNode")
                .Back();
                //
                // .AddC
        }

        void Update()
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            Tree.Tick(Time.deltaTime);
        }
    }
}