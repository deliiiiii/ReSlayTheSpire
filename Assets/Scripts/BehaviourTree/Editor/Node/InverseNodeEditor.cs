using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class InverseNodeEditor : NodeBaseEditor<InverseNode>
    {
        public override Node CreateNodeInGraph()
        {
            var node = new InverseNodeEditor
            {
                title = "Inverse Node",
                viewDataKey = "InverseNode_001",
                nodeBase = new InverseNode(),
            };
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            node.inputContainer.Add(inputPort);

            var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Output";
            outputPort.tooltip = typeof(bool).ToString();
            node.outputContainer.Add(outputPort);
            return node;
        }
    }
}

