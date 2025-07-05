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

        protected override void DrawPort()
        {
            base.DrawPort();
            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(IACDNodeEditor<ACDNode>));
            OutputPort.portName = "Dec ↓";
            OutputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(OutputPort);
        }
    }
}