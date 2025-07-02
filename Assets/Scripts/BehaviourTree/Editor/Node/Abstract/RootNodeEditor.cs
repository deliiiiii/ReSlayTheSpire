using System.Linq;
using Sirenix.Utilities;
using UnityEditor.Experimental.GraphView;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public class RootNodeEditor : NodeBaseEditor<RootNode>
    {
        Port outputPort; 
        protected override void DrawPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(IACDNodeEditor<ACDNode>));
            outputPort.portName = "Root ↓";
            outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(outputPort);
            
        }

        public override void OnConstructTree()
        {
            OutEdges.ForEach(port =>
            {
                if (port.input.node is not IACDNodeEditor<ACDNode> childNode)
                    return;
                MyDebug.Log($"Root {NodeBase.NodeName} AddChild {childNode.NodeBase.NodeName}");
                NodeBase.ChildNode = childNode.NodeBase;
                childNode.OnConstructTree();
            });
        }
    }
}