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
        protected override void DrawPort()
        {
            base.DrawPort();
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(IACDNodeEditor<ACDNode>));
            outputPort.portName = "Dec ↓";
            outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}