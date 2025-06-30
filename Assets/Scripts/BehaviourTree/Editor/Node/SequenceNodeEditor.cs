using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SequenceNodeEditor : NodeBaseEditor<SequenceNode>
    {
        public SequenceNodeEditor()
        {
            title = "Sequence Node";
            viewDataKey = "SequenceNode_001";
            nodeBase = new SequenceNode();
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Output";
            outputPort.tooltip = typeof(bool).ToString();
            outputContainer.Add(outputPort);
        }
    }
}



