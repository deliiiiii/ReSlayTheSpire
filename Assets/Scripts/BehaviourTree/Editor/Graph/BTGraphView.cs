using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sirenix.Utilities;
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

        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    if (edge.output.node is INodeBaseEditor<NodeBase> parentEditor && 
                        edge.input.node is INodeBaseEditor<NodeBase> childEditor)
                    {
                        MyDebug.Log($"Adding child {childEditor.NodeBase.NodeName} to parent {parentEditor.NodeBase.NodeName}");
                        parentEditor.NodeBase.AddChild(childEditor.NodeBase);
                    }
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    if (edge.output.node is INodeBaseEditor<NodeBase> parentEditor && 
                        edge.input.node is INodeBaseEditor<NodeBase> childEditor)
                    {
                        MyDebug.Log($"Removing child {childEditor.NodeBase.NodeName} from parent {parentEditor.NodeBase.NodeName}");
                        parentEditor.NodeBase.RemoveChild(childEditor.NodeBase);
                    }
                }
            }
            ConstructTree();
            return graphViewChange;
        }

        void ConstructTree()
        {
            //找到图中第一个没有输入 但有输出的节点作为根节点
            var rootNodeSonClass = nodes.Where(node => node.GetType().InheritsFrom(typeof(NodeBaseEditor<>)))
                                .FirstOrDefault(n => 
                                    //错误用法
                                    // n.inputContainer.Children().Sum(x => x.childCount) == 0
                                    !n.inputContainer.Q<Port>()?.connections.Any() ??
                                    n.outputContainer.Q<Port>()?.connections.Any() ??
                                    false);
            if (rootNodeSonClass is not INodeBaseEditor<NodeBase> rootNodeInterface)
            {
                MyDebug.LogError("No root node found in the graph.");
                return;
            }
            TreeTest.CreateByRoot(rootNodeInterface.NodeBase);
            rootNodeInterface.AddInEditorChildren();
        }
        
        // void ConstructTree()
        // {
        //     var tree = new Tree();
        //     //找到图中第一个没有输入 但有输出的节点作为根节点
        //     var rootNodeSonClass = nodes.Where(node => node.GetType().InheritsFrom(typeof(NodeBaseEditor<>)))
        //         .FirstOrDefault(n => 
        //             //错误用法
        //             // n.inputContainer.Children().Sum(x => x.childCount) == 0
        //             !n.inputContainer.Q<Port>().connections.Any()
        //             && n.outputContainer.Q<Port>().connections.Any());
        //     if (rootNodeSonClass is not NodeBaseEditor<NodeBase> rootNodeGeneral)
        //     {
        //         //找到图中第一个没有输入 也没有输出的节点作为根节点
        //         rootNodeSonClass = nodes.Where(node => node.GetType().InheritsFrom(typeof(NodeBaseEditor<>)))
        //             .FirstOrDefault(n => 
        //                 !n.inputContainer.Q<Port>().connections.Any()
        //                 && !n.outputContainer.Q<Port>().connections.Any());
        //         rootNodeGeneral = rootNodeSonClass as NodeBaseEditor<NodeBase>;
        //         if (rootNodeGeneral == null)
        //         {
        //             MyDebug.LogError("No root node found in the graph.");
        //             return;
        //         }
        //     }
        //     tree.Root = rootNodeGeneral.NodeBase;
        //     tree.Root.Tree = tree;
        //     AddChildren(rootNodeGeneral);
        //
        //     TreeTest.StaticTree = tree;
        // }
        
    }
}