using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
    }
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public T NodeBase { get; protected set; }
    }
}