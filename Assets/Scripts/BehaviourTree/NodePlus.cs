namespace BehaviourTree
{
    public interface INodePlus<out TParent, out TThis,out TChild>
        where TParent : NodeBase
        where TThis : NodeBase
        where TChild : NodeBase
    {
        public TParent Parent { get; }
        public TThis Node { get; }
        public TChild Child{ get; }
    }
    public class NodePlus<TParent, TThis, TChild> : INodePlus<TParent, TThis, TChild>
        where TThis : NodeBase
        where TParent : NodeBase
        where TChild : NodeBase
    {
        public TParent Parent { get; set; }
        public TThis Node { get; }
        public TChild Child => Node.GetChild() as TChild;

        public NodePlus(TThis node)
        {
            this.Node = node;
        }
        
        public NodePlus(TParent parent, TThis node)
        {
            Node = node;
            Parent = parent;
        }
    }
}