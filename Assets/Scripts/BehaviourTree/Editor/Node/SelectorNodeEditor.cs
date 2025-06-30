using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SelectorNodeEditor : NodeBaseEditor<SelectorNode>
    {
        public override Node CreateNodeInGraph()
        {
            var node = new SelectorNodeEditor
            {
                title = "Selector Node",
                viewDataKey = "SelectorNode_001",
                nodeBase = new SelectorNode(),
            };
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            node.inputContainer.Add(inputPort);

            var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Output";
            outputPort.tooltip = typeof(bool).ToString();
            node.outputContainer.Add(outputPort);
            return node;
        }
    }
}