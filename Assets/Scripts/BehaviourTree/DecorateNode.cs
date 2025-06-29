using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : NodeBase
    {
        public NodeBase Child;

        public override NodeBase GetChild()
        {
            return Child;
        }

        public override EState OnTick(float dt)
        {
            // 1变成0 0变成1
            return ~Child?.OnTick(dt) ?? EState.Failed;
        }
        public override void OnFail()
        {
            Child?.OnFail();
        }
    }
}