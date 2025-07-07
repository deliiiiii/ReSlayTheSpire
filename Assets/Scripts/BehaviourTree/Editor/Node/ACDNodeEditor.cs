using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using Sirenix.Utilities;
using UnityEngine;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    
    public abstract class ACDNodeEditor<T>
        : NodeBaseEditor<T> where T : NodeBase
    {
        protected ACDNodeEditor(T nodeBase) : base(nodeBase)
        {
        }
        // protected override void DrawPort()
        // {
        //     InputParentPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(INodeBaseEditor<NodeBase>));
        //     InputParentPort.portName = "Parent ↑";
        //     InputParentPort.tooltip = typeof(INodeBaseEditor<NodeBase>).ToString();
        //     inputContainer.Add(InputParentPort);
        //     
        //     InputGuardingPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(GuardNodeEditor));
        //     InputGuardingPort.portName = "Guarded By ↑";
        //     InputGuardingPort.tooltip = typeof(GuardNodeEditor).ToString();
        //     inputContainer.Add(InputGuardingPort);
        // }
    }
}