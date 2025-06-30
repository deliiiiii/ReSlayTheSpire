using System;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        public Tree Tree = new();
        public bool TestBool = true;

        public static Tree StaticTree
        {
            get => Instance.Tree;
            set => Instance.Tree = value;
        }

        void Awake()
        {
            CreateTree();
        }

        void Update()
        {
            Tick(Time.deltaTime);
        }

         void CreateTree()
         {
            Application.targetFrameRate = 30;
            Tree.CreateVir<SelectorNode>().SetName("RootNode")
                .AddChild(new SequenceNode())
                .ToChild().SetName("SeqTrue")
                    .SetGuard(() => TestBool)
                    .AddChild(new ActionNodeDebug("Start")).SetChildName("StartNode")
                    .AddChild(new ActionNodeDelay(1.2f)).SetChildName("DelayNodeTrue")
                    .AddChild(new ActionNodeSet<bool>(false, tar => TestBool = tar)).SetChildName("SetterTrue")
                    .AddChild(new ActionNodeDebug("End")).SetChildName("EndNode")
                .Back()
                .AddChild(new SequenceNode())
                .ToChild().SetName("SeqFalse")
                    .AddChild(new ActionNodeDelay(1.5f)).SetChildName("DelayNodeFalse")
                    .AddChild(new ActionNodeSet<bool>(true, tar => TestBool = tar)).SetChildName("SetterFalse")
                .Back();
            
            // Tree.CreateVir<SelectorNode>()
            //     .AddChildStay(new SelectorNode())
            //     .ToChild()
            //         .AddChildStay(new SequenceNode())
            //         .ToChild()
            //             .AddChildStay(new SelectorNode())
            //             .ToChild()
            //                 .AddChildStay(new SequenceNode())
            //                 .ToChild();
        }
        void Tick(float dt)
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            Tree.Tick(dt);
        }
    }
}