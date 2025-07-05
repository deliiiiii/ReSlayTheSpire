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
        public Port InputACDPort { get; }
        public Port InputGuardPort { get; }
        Port OutputPort { get;}
        public Edge ConnectChildNodeEditor(IACDNodeEditor<ACDNode> childNodeEditor)
        {
            return OutputPort.ConnectTo(childNodeEditor.InputACDPort);
        }
        public Edge ConnectGuardNodeEditor(GuardNodeEditor guardNodeEditor)
        {
            return InputGuardPort.ConnectTo(guardNodeEditor.outputPort);
        }
    }
    
    public abstract class ACDNodeEditor<T>
        : NodeBaseEditor<T>, IACDNodeEditor<T> where T : ACDNode
    {
        protected ACDNodeEditor(T nodeBase) : base(nodeBase)
        {
        }
        
        
        
        public Port InputACDPort { get; protected set; }
        public Port InputGuardPort{ get; protected set; }
        public Port OutputPort { get; protected set; }

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
            InputACDPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(IACDNodeEditor<ACDNode>));
            InputACDPort.portName = "Parent ↑";
            InputACDPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            inputContainer.Add(InputACDPort);
            
            InputGuardPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(GuardNodeEditor));
            InputGuardPort.portName = "Guarded By ↑";
            InputGuardPort.tooltip = typeof(GuardNodeEditor).ToString();
            inputContainer.Add(InputGuardPort);
        }
        
        public override void OnRefreshTree()
        {
            NodeBase.ClearChildren();
            childsEditor.ForEach(childEditor =>
            {
                // MyDebug.Log($"Editor : {NodeBase.NodeName} AddChild {childEditor.NodeBase.NodeName}");
                NodeBase.AddChild(childEditor.NodeBase);
                childEditor.OnRefreshTree();
            });
            
            NodeBase.GuardNode = null;
            // MyDebug.Log($"Editor : {NodeBase.NodeName} AddGuard {guard?.NodeName ?? "null"}");
            NodeBase.GuardNode = guard;
        }
        
        public override void OnSave()
        {
            base.OnSave();
            AssetDataBaseExtension.SafeAddSubAsset(guard, this.NodeBase);
            guardEditor?.OnSave();
            childsEditor.ForEach(childEditor =>
            {
                AssetDataBaseExtension.SafeAddSubAsset(childEditor.NodeBase, this.NodeBase);
                childEditor.OnSave();
            });
        }
    }
}