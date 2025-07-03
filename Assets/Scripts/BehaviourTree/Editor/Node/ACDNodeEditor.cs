using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using Sirenix.Utilities;
using UnityEngine;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public interface IACDNodeEditor<out T> : INodeBaseEditor<T> where T : ACDNode
    {
    }
    
    public abstract class ACDNodeEditor<T>
        : NodeBaseEditor<T>, IACDNodeEditor<T> where T : ACDNode
    {
        Port inputACDPort;
        Port inputGuardPort;
        protected Port outputPort;

        IEnumerable<IACDNodeEditor<ACDNode>> childsEditor => 
            OutEdges
                .Where(port => port.input.node is IACDNodeEditor<ACDNode>)
                .Select(port => port.input.node as IACDNodeEditor<ACDNode>);
        GuardNodeEditor guardEditor =>
            InEdges
                .Where(port => port.output.node is GuardNodeEditor)
                .Select(port => port.output.node as GuardNodeEditor)
                .FirstOrDefault();

        [CanBeNull] GuardNode guard => guardEditor?.NodeBase;
        
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
            NodeBase.ClearChildren();
            childsEditor.ForEach(childEditor =>
            {
                MyDebug.Log($"Editor : {NodeBase.NodeName} AddChild {childEditor.NodeBase.NodeName}");
                NodeBase.AddChild(childEditor.NodeBase);
                childEditor.OnConstructTree();
            });
            
            NodeBase.GuardNode = null;
            MyDebug.Log($"Editor : {NodeBase.NodeName} AddGuard {guard?.NodeName}");
            NodeBase.GuardNode = guard;
        }
        
        public override void OnSave()
        {
            AssetDataBaseExtension.SafeAddSubAsset(guard, this.NodeBase);
            childsEditor.ForEach(childEditor =>
            {
                AssetDataBaseExtension.SafeAddSubAsset(childEditor.NodeBase, this.NodeBase);
                childEditor.OnSave();
            });
        }
        
    }
}