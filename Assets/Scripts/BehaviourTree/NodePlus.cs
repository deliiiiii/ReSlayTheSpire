namespace BehaviourTree
{
    public interface INodePlus<out TParent, out TThis, out TChild>
        where TParent : NodeBase
        where TThis : NodeBase
        where TChild : NodeBase
    {
        public TParent Parent { get; }
        public TThis Node { get; }
        public NodeBase GetChild();
    }
    public class NodePlus<TParent, TThis, TChild> : NodeBase, INodePlus<TParent, TThis, TChild>
        where TThis : NodeBase
        where TParent : NodeBase
        where TChild : NodeBase
    {
        public TParent Parent { get; set; }
        public TThis Node { get; }
        public override NodeBase GetChild()
        {
            return Node.GetChild();
        }

        public NodePlus(TThis node)
        {
            this.Node = node;
            // Parent = NullNode as TParent;
        }
        
        public NodePlus(TParent parent, TThis node)
        {
            Node = node;
            Parent = parent;
        }
        
       
        
        public override bool OnTick(float dt)
        {
            return Node.OnTick(dt);
        }

        public override void OnFail()
        {
            Node.OnFail();
        }
    }
}