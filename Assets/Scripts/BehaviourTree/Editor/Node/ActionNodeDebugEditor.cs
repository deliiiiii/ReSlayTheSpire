using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class ActionNodeDebugEditor : NodeBaseEditor<ActionNodeConcrete>
    {
        public override Node CreateNodeInGraph()
        {
            var node = new ActionNodeDebugEditor
            {
                title = "Action Node Debug",
                viewDataKey = "ActionNodeDebug_001",
                nodeBase = new ActionNodeConcrete("Debug Message"),
            };
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            node.inputContainer.Add(inputPort);
            
            return node;
        }
    }
}