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
                .AddChildStayPlus(new SequenceNode()).SetName("Se1")
                .SetGuard(() => TestBool)
                .ToChild();
                
                //!!!!    
                // .AddChildStayPlus(new ActionNodeDebug("Start")); //.SetChildNamePlus("StartNode")
            
            
            //     .AddChildStayPlus(new ActionNodeDelay(2f)).SetChildNamePlus("DelayNode")
            //     .AddChildStayPlus(new ActionNodeSet<bool>(false, tar => TestBool = tar)).SetChildNamePlus("Setter")
            //     .AddChildStayPlus(new ActionNodeDebug("End")).SetChildNamePlus("EndNode")
            // .BackPlus();
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