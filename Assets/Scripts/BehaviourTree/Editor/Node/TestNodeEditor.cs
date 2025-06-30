using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class TestNodeEditor : NodeBaseEditor<TestNode>
    {
        public TestNodeEditor()
        {
            title = "Test Node";
            viewDataKey = "TestNode_001";
            nodeBase = new TestNode
            {
                TestInt = 42,
            };
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
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