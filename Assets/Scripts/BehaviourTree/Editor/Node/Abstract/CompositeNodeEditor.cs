using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class CompositeNodeEditor : NodeBaseEditor<CompositeNode>
    {
        protected override void DrawPort()
        {
            base.DrawPort();
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Com ↓";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}