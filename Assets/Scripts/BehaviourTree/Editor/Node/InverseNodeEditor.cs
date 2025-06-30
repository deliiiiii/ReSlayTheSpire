using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class InverseNodeEditor : NodeBaseEditor<InverseNode>
    {
        public InverseNodeEditor()
        {
            title = "Inverse Node";
            viewDataKey = "InverseNode_001";
            nodeBase = new InverseNode();
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Output";
            outputPort.tooltip = typeof(bool).ToString();
            outputContainer.Add(outputPort);
        }
    }
}

