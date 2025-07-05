using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public class RootNodeEditor : NodeBaseEditor<RootNode>
    {
        public RootNodeEditor(RootNode nodeBase) : base(nodeBase)
        {
        }
        
        Port outputPort;
        IACDNodeEditor<ACDNode> childEditor =>
            OutEdges
                .Where(port => port.input.node is IACDNodeEditor<ACDNode>)
                .Select(port => port.input.node as IACDNodeEditor<ACDNode>)
                .FirstOrDefault();
        
        protected override void DrawPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(IACDNodeEditor<ACDNode>));
            outputPort.portName = "Root ↓";
            outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(outputPort);
            
        }

        public override void OnRefreshTree()
        {
            NodeBase.ChildNode = null;
            if (childEditor != null)
            {
                MyDebug.Log($"Root {NodeBase.NodeName} AddChild {childEditor.NodeBase.NodeName}");
                NodeBase.ChildNode = childEditor.NodeBase;
                childEditor.OnRefreshTree();
            }
        }
        public override void OnSave()
        {
            base.OnSave();
            if (childEditor != null)
            {
                AssetDataBaseExtension.SafeAddSubAsset(childEditor.NodeBase, this.NodeBase);
                childEditor.OnSave();
            }
        }
    }
}