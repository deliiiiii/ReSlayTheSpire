using System;

namespace BehaviourTree
{
    public abstract class DecorateNode : NodeBase
    {
        public NodeBase Child;
        public override NodeBase ToChild()
        {
            return Child;
        }
        
        public override NodeBase AddChild(NodeBase child)
        {
            if (Child != null)
            {
                MyDebug.LogError("InverseNode can only have one child.");
                return this;
            }
            Child = child;
            return this;
        }
    }
    
    [Serializable]
    public class InverseNode : DecorateNode
    {
        public NodeBase Child;
        public override EState OnTick(float dt)
        {
            // 1变成0 0变成1
            return ~Child?.OnTick(dt) ?? EState.Failed;
        }
        public override void OnFail()
        {
            Child?.OnFail();
        }
        public override NodeBase ToChild()
        {
            return Child;
        }

        
    }
}