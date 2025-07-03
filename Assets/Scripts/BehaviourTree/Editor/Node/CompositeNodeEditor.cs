using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class CompositeNodeEditor : ACDNodeEditor<CompositeNode>
    {
        protected override void DrawPort()
        {
            base.DrawPort();
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                typeof(IACDNodeEditor<ACDNode>));
            outputPort.portName = "Com ↓";
            outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}