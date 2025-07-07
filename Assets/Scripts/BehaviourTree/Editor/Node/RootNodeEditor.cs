using UnityEditor.Experimental.GraphView;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public class RootNodeEditor : NodeBaseEditor<RootNode>
    {
        public RootNodeEditor(RootNode nodeBase) : base(nodeBase)
        {
        }
        
        // protected override void DrawPort()
        // {
        //     OutputChildsPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
        //         typeof(INodeBaseEditor<NodeBase>));
        //     OutputChildsPort.portName = "Root ↓";
        //     OutputChildsPort.tooltip = typeof(INodeBaseEditor<NodeBase>).ToString();
        //     outputContainer.Add(OutputChildsPort);
        // }
    }
}