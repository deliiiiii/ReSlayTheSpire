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
            return ports.ToList()
                .Where(port => 
                    port.direction != startPort.direction 
                    && port.portType.IsAssignableFrom(startPort.portType))
                .ToList();
        }
        
        public void DrawNodeEditor(Type nodeEditorType)
        {
            //利用反射调用构造函数
            var ins = Activator.CreateInstance(nodeEditorType);
            
            var nodeI = ins as INodeBaseEditor<NodeBase>;
            nodeI.OnChangeTypeEvent += newNodeEditorType =>
            {
                MyDebug.Log($" 22 {newNodeEditorType.Name}");
                MyDebug.Log($"select: changed to {newNodeEditorType.Name[..^2]}<{newNodeEditorType.GetGenericArguments()[0]}>");
                nodeI.SetNodeBase(newNodeEditorType.GetGenericArguments()[0]);
            };

            var node = ins as Node;
            node.SetPosition(new Rect(100, 100, 200, 150));
            node.RefreshPorts();
            node.expanded = true;
            node.RefreshExpandedState();
            AddElement(node);
        }
        public void DrawNodeEditor<T>() where T : Node
        {
            DrawNodeEditor(typeof(T));
        }

        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.edgesToCreate?
                .ForEach(edge =>
                {
                    if (edge.output.node is not INodeBaseEditor<NodeBase> parentEditor
                        || edge.input.node is not INodeBaseEditor<NodeBase> childEditor)
                    {
                        return;
                    }
                    // MyDebug.Log($"Adding child {childEditor.NodeBase.NodeName} to parent {parentEditor.NodeBase.NodeName}");
                    parentEditor.NodeBase.AddChild(childEditor.NodeBase);
                });
            
            graphViewChange.elementsToRemove?
                .OfType<Edge>()
                .ForEach(edge =>
                {
                    if (edge.output.node is not INodeBaseEditor<NodeBase> parentEditor
                        || edge.input.node is not INodeBaseEditor<NodeBase> childEditor)
                    {
                        return;
                    }
                    // MyDebug.Log($"Removing child {childEditor.NodeBase.NodeName} from parent {parentEditor.NodeBase.NodeName}");
                    parentEditor.NodeBase.RemoveChild(childEditor.NodeBase);
                });
            ConstructTree();
            return graphViewChange;
        }

        void ConstructTree()
        {
            //找到图中第一个没有输入 但有输出的节点作为根节点
            var rootNodeSonClass = nodes
                    .Where(node => 
                        node.GetType().InheritsFrom(typeof(NodeBaseEditor<>)))
                    .FirstOrDefault(n => 
                        //错误用法
                        // n.inputContainer.Children().Sum(x => x.childCount) == 0
                        n.inputContainer.Q<Port>() == null || !n.inputContainer.Q<Port>().connections.Any() &&
                        n.outputContainer.Q<Port>() != null &&
                        n.outputContainer.Q<Port>().connections.Any()
                        );
            if (rootNodeSonClass is not INodeBaseEditor<NodeBase> rootNodeInterface)
            {
                // MyDebug.LogError("No root node found in the graph.");
                return;
            }

            var inCount = rootNodeSonClass.inputContainer.Q<Port>()?.connections.Count();
            var outCount = rootNodeSonClass.outputContainer.Q<Port>()?.connections.Count();
            MyDebug.Log($"Root node found: {rootNodeInterface.NodeBase.NodeName} in : {inCount} out : {outCount}");
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