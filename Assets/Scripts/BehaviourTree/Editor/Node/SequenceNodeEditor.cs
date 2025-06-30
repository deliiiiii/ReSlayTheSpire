using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SequenceNodeEditor : NodeBaseEditor<SequenceNode>
    {
        List<NodeBaseEditor<NodeBase>> childList;
        public SequenceNodeEditor()
        {
            title = "Sequence →";
            viewDataKey = "SequenceNode_001";
            childList = new List<NodeBaseEditor<NodeBase>>();
            NodeBase = new SequenceNode();
            
            
            var inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Seq ↓";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Parent ↑";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}



