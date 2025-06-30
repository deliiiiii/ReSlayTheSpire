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
            return ~curChild?.OnTick(dt) ?? EState.Failed;
        }
        public override void OnFail()
        {
            curChild?.OnFail();
        }
    }
}