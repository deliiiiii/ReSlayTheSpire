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
            //利用反射调用构造函数
            var ins = Activator.CreateInstance(nodeType);
            var node = ins as Node;
            // if (invoke is not Node node)
            // {
            //     Debug.LogError($"Node type {nodeType} does not have a CreateNodeInGraph method.");
            //     return;
            // }
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
    }
}