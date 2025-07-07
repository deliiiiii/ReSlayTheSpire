using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class CompositeNodeEditor : ACDNodeEditor<CompositeNode>
    {
        public CompositeNodeEditor(CompositeNode nodeBase) : base(nodeBase)
        {
        }

        protected override void DrawPort()
        {
            base.DrawPort();
            OutputChildsPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                typeof(INodeBaseEditor<NodeBase>));
            OutputChildsPort.portName = "Com ↓";
            OutputChildsPort.tooltip = typeof(INodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(OutputChildsPort);
        }
    }
}