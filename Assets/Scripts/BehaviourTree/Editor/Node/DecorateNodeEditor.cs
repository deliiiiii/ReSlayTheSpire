using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class DecorateNodeEditor : ACDNodeEditor<DecorateNode>
    {
        public DecorateNodeEditor(DecorateNode nodeBase) : base(nodeBase)
        {
        }

        // protected override void DrawPort()
        // {
        //     base.DrawPort();
        //     OutputChildsPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
        //         typeof(INodeBaseEditor<NodeBase>));
        //     OutputChildsPort.portName = "Dec ↓";
        //     OutputChildsPort.tooltip = typeof(INodeBaseEditor<NodeBase>).ToString();
        //     outputContainer.Add(OutputChildsPort);
        // }
    }
}