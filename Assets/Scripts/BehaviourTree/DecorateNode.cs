using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : NodeBase
    {
        public NodeBase Child;

        public override bool OnTick(float dt)
        {
            return Child?.OnTick(dt) ?? true;
        }
        public override void OnFail()
        {
            Child?.OnFail();
        }
    }
}