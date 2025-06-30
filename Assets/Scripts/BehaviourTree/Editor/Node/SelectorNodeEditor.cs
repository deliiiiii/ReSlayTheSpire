using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class SelectorNodeEditor : NodeBaseEditor<SelectorNode>
    {
        public SelectorNodeEditor()
        {
            title = "Selector Node";
            viewDataKey = "SelectorNode_001";
            nodeBase = new SelectorNode();
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