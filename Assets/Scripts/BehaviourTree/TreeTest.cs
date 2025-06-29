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
                .AddChildStay(new SequenceNode()).SetChildName("SeqTrue")
                .SetGuard(() => TestBool)
                .ToChild()
                    .AddChildStay(new ActionNodeDebug("Start")).SetChildName("StartNode")
                    .AddChildStay(new ActionNodeDelay(2f)).SetChildName("DelayNodeTrue")
                    .AddChildStay(new ActionNodeSet<bool>(false, tar => TestBool = tar)).SetChildName("SetterTrue")
                    .AddChildStay(new ActionNodeDebug("End")).SetChildName("EndNode")
                .Back()
                .AddChildStay(new SequenceNode()).SetChildName("SeqFalse")
                .ToChild()
                    .AddChildStay(new ActionNodeDelay(3f)).SetChildName("DelayNodeFalse")
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