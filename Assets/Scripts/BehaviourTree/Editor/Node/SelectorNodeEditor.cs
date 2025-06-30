using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SelectorNodeEditor : NodeBaseEditor<SelectorNode>
    {
        public SelectorNodeEditor()
        {
            title = "Selector ?";
            viewDataKey = "SelectorNode_001";
            NodeBase = new SelectorNode();
            
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