using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        public InverseNode(){}
        public override EState OnTick(float dt)
        {
            // 1变成0 0变成1
            return ~ToChild()?.OnTick(dt) ?? EState.Failed;
        }
        public override void OnFail()
        {
            ToChild()?.OnFail();
        }
    }

    public class NothingNode : DecorateNode
    {
        public NothingNode() { }
        public override EState OnTick(float dt)
        {
            // 什么都不做
            return EState.Succeeded;
        }
        public override void OnFail()
        {
            ToChild()?.OnFail();
        }
    }
}