using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public abstract class NodeBaseEditor<T> : Node where T : NodeBase
    {
        protected T nodeBase;
        public abstract Node CreateNodeInGraph();
    }
}