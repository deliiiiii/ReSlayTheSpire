using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class TestNode : NodeBase
    {
        public int TestInt;
        public SelectorNode SelectorNode;
        public ActionNode ActionNode;
        
        public NodeBase NodeBase;
        public override EState OnTick(float dt)
        {
            throw new System.NotImplementedException();
        }

        public override void OnFail()
        {
            throw new System.NotImplementedException();
        }
        
        public override NodeBase AddChild(NodeBase child)
        {
            throw new System.NotImplementedException();
        }
    }
}