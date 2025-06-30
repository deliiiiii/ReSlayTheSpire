using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public abstract class CompositeNodeEditor<T> : NodeBaseEditor<T> where T : CompositeNode
    {
        public CompositeNodeEditor()
        {
            viewDataKey = "SequenceNode_001";
            
            NodeBase = Activator.CreateInstance<T>();
            title = typeof(T).Name;
            
            
            var outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Parent ↑";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
            
            var inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Com ↓";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);

        }
    }
}