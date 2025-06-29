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

        public override bool OnTick(float dt)
        {
            return Child?.OnTick(dt) ?? true;
        }
        public override void OnFail()
        {
            Child?.OnFail();
        }
    }
    
    [Serializable]
    public class NullNode : NodeBase
    {
        public override NodeBase GetChild()
        {
            return null; // NullNode has no child
        }

        public override bool OnTick(float dt)
        {
            return true; // Always returns true, does nothing
        }

        public override void OnFail()
        {
            // Does nothing on fail
        }
    }
}