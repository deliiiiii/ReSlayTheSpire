using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BTGraphView : GraphView
    {
        // protected override bool canCopySelection => selection.OfType<Node>().Any();
        // protected override bool canCutSelection => canCopySelection;
        // protected override bool canPaste => true;
        // protected override bool canDuplicateSelection => canCopySelection;
        public BTGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            // var compatibleTypes = new ;

            foreach (var port in ports.ToList())
            {
                if (port.direction == startPort.direction ||
                    !port.portType.IsAssignableFrom(startPort.portType))
                    continue;
                compatiblePorts.Add(port);
                // if (compatibleTypes.Contains(port.portType))
                // {
                //     
                // }
            }

            return compatiblePorts;
        }
        
        public void CreateNode(Type nodeType)
        {
            Node node;
            if(nodeType == typeof(TestNode))
            {
                node = CreateTestNode();
            }
            else if(nodeType == typeof(SequenceNode))
            {
                node = CreateSequenceNode();
            }
            else
            {
                Debug.LogError($"Unsupported node type: {nodeType}");
                return;
            }
            node.SetPosition(new Rect(100, 100, 200, 150));
            node.RefreshPorts();
            node.expanded = true;
            node.RefreshExpandedState();
            AddElement(node);
        }
        public void CreateNode<T>() where T : Node
        {
            CreateNode(typeof(T));
        }

        static TestNode CreateTestNode()
        {
            var node = new TestNode
            {
                title = "Test Node",
                viewDataKey = "TestNode_001",
                TestInt = 42,
            };
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Input";
            inputPort.tooltip = typeof(bool).ToString();
            node.inputContainer.Add(inputPort);
            
            var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Output";
            outputPort.tooltip = typeof(bool).ToString();
            node.outputContainer.Add(outputPort);
            return node;
        }

        static SequenceNode CreateSequenceNode()
        {
            var node = new SequenceNode
            {
                title = "Sequence Node",
                viewDataKey = "SequenceNode_001",
            };

            return node;
        }
    }
}