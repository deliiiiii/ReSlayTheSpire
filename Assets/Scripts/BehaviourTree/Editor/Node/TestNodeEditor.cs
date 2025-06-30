using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class TestNodeEditor : NodeBaseEditor<TestNode>
    {
        public override Node CreateNodeInGraph()
        {
            var node = new TestNodeEditor
            {
                title = "Test Node",
                viewDataKey = "TestNode_001",
                nodeBase = new TestNode
                {
                    TestInt = 42,
                },
            };
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
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