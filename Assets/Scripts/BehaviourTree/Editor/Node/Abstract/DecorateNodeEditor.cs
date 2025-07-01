using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class DecorateNodeEditor : NodeBaseEditor<DecorateNode>
    {
        public DecorateNodeEditor()
        {
            CreateOutputPort();
        }

        void CreateOutputPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Com ↓";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}