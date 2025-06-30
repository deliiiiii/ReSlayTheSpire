using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SequenceNodeEditor : NodeBaseEditor<SequenceNode>
    {
        public SequenceNodeEditor()
        {
            title = "Sequence →";
            viewDataKey = "SequenceNode_001";
            
            NodeBase = new SequenceNode();
            
            var inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Parent ↑";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);
            
            var outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Seq ↓";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);

        }
    }
}



