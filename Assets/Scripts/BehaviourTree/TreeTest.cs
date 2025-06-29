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
                .AddChildStay(new SequenceNode())
                .ToChild().SetName("SeqTrue")
                    .SetGuard(() => TestBool)
                    .AddChildStay(new ActionNodeDebug("Start")).SetChildName("StartNode")
                    .AddChildStay(new ActionNodeDelay(1.2f)).SetChildName("DelayNodeTrue")
                    .AddChildStay(new ActionNodeSet<bool>(false, tar => TestBool = tar)).SetChildName("SetterTrue")
                    .AddChildStay(new ActionNodeDebug("End")).SetChildName("EndNode")
                .Back()
                .AddChildStay(new SequenceNode())
                .ToChild().SetName("SeqFalse")
                    .AddChildStay(new ActionNodeDelay(1.5f)).SetChildName("DelayNodeFalse")
                    .AddChildStay(new ActionNodeSet<bool>(true, tar => TestBool = tar)).SetChildName("SetterFalse")
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