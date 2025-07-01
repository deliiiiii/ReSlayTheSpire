using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public abstract class CompositeNodeEditor<T> : NodeBaseEditor<T> where T : CompositeNode
    {
        public CompositeNodeEditor()
        {
            viewDataKey = "SequenceNode_001";
            CreateOutputPort();
        }

        void CreateOutputPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Com ↓";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}