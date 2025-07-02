using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Sirenix.Utilities;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public interface IACDNodeEditor<out T> : INodeBaseEditor<T> where T : ACDNode
    {
        /// <summary>
        /// 添加子节点到编辑器中
        /// </summary>
        // public void AddInEditorChildren();
    }
    
    public abstract class ACDNodeEditor<T> : NodeBaseEditor<T>, IACDNodeEditor<T> where T : ACDNode
    {
        Port inputACDPort;
        Port inputGuardPort;
        protected Port outputPort;

        protected override void DrawPort()
        {
            inputACDPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(IACDNodeEditor<ACDNode>));
            inputACDPort.portName = "Parent ↑";
            inputACDPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            inputContainer.Add(inputACDPort);
            
            inputGuardPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(GuardNodeEditor));
            inputGuardPort.portName = "Guarded By ↑";
            inputGuardPort.tooltip = typeof(GuardNodeEditor).ToString();
            inputContainer.Add(inputGuardPort);
        }
        
        public override void OnConstructTree()
        {
            OutEdges.ForEach(port =>
            {
                if (port.input.node is not IACDNodeEditor<ACDNode> childNode)
                    return;
                MyDebug.Log($"{NodeBase.NodeName} AddChild {childNode.NodeBase.NodeName}");
                NodeBase.AddChild(childNode.NodeBase);
                childNode.OnConstructTree();
            });
            
            InEdges.ForEach(port =>
            {
                if (port.output.node is not GuardNodeEditor guardNodeEditor)
                    return;
                NodeBase.GuardNode = guardNodeEditor.NodeBase;
                MyDebug.Log($"{NodeBase.NodeName} AddGuard {guardNodeEditor.NodeBase.NodeName}");
            });
            
        }
    }
}