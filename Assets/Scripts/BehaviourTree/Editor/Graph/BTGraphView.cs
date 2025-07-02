using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sirenix.Utilities;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BTGraphView : GraphView
    {
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
            var iNode = ins as INodeBaseEditor<NodeBase>;
            iNode.OnNodeBaseChanged += _ => OnGraphViewChanged(default);
            
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
            MyDebug.LogWarning($"OnGraphViewChanged");
            nodes.ForEach(node =>
            {
                if (node is not IACDNodeEditor<ACDNode> nodeEditor)
                    return;
                nodeEditor.NodeBase.ClearChildren();
            });
            Observable.NextFrame().Subscribe(_ => ConstructTree());
            return graphViewChange;
        }
        void ConstructTree()
        {
            //找到图中第一个没有输入 但有输出的节点作为根节点
            var firstNode = nodes
                .Where(node =>
                {
                    if (node is not IACDNodeEditor<ACDNode> acdNodeEditor)
                        return false;
                    return acdNodeEditor.InEdgesCount == 0 && acdNodeEditor.OutEdgesCount > 0;
                }).First();
                    
            if (firstNode is not IACDNodeEditor<ACDNode> rootNodeEditor)
            {
                MyDebug.Log("No root node found in the graph.");
                return;
            }
            MyDebug.Log($"Root node found: {rootNodeEditor.NodeBase.NodeName} in : {rootNodeEditor.InEdgesCount} out : {rootNodeEditor.OutEdgesCount}");
            TreeTest.CreateByRoot(rootNodeEditor.NodeBase);
            rootNodeEditor.OnConstructTree();
        }
    }
}