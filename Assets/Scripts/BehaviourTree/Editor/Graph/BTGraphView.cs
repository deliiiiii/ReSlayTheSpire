using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sirenix.Utilities;
using UniRx;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BTGraphView : GraphView
    {
        string s1 = "DataTree";
        string s2 = nameof(RootNode);
        string path => $"Assets/{s1}/{s2}.asset";
        RootNodeEditor rootEditor;

        static RootNode rootNode
        {
            get => TreeTest.Root;
            set => TreeTest.Root = value;
        }
        public BTGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = OnGraphViewChanged;
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

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            MyDebug.LogWarning($"OnGraphViewChanged");
            nodes.ForEach(node =>
            {
                if (node is not IACDNodeEditor<ACDNode> nodeEditor)
                    return;
                nodeEditor.NodeBase.ClearChildren();
            });
            Observable.NextFrame().Subscribe(_ => RefreshTree());
            return graphViewChange;
        }
        void RefreshTree()
        {
            rootEditor = nodes.FirstOrDefault(node => node is RootNodeEditor) as RootNodeEditor;
            if (rootEditor == null)
            {
                MyDebug.Log("No ROOT node found in the graph.");
                return;
            }
            rootEditor.OnRefreshTree();
            rootNode = rootEditor.NodeBase;
        }

        void ClearGraph()
        {
            nodes.ForEach(RemoveElement);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcrete"> 如ActionNodeDebug，具体Node类</param>
        NodeBaseEditor<T> CreateNodeEditorOnLoad<T>(T nodeConcrete) where T : NodeBase
        {
            var nodeConcreteType = nodeConcrete.GetType();
            // // SequenceNode -> CompositeNode, ActionNodeDebug -> ActionNode
            // while (!nodeConcreteType!.IsAbstract)
            // {
            //     nodeConcreteType = nodeConcreteType.BaseType;
            // }
            if(Activator.CreateInstance(TypeCache.GetEditorBySubType(nodeConcreteType)) is not NodeBaseEditor<T> ins)
            {
                MyDebug.LogError($"Failed to create NodeBaseEditor for {nodeConcreteType.Name}");
                return null;
            }
            MyDebug.Log($"CreateNodeEditorOnLoad: {ins.GetType().Name} for {nodeConcreteType.Name}");
            
            AddElement(ins);
            ins.OnLoad(nodeConcrete);
            return ins;
        }
        
        void RecursivelyReadChildNodes(NodeBase nodeBase)
        {
            if (nodeBase is not IHasChild hasChild)
                return;
            foreach (var child in hasChild.ChildNodes)
            {
                CreateNodeEditorOnLoad(child);
                RecursivelyReadChildNodes(child);
            }
        }
        
        public void Load()
        {
            //TODO 运行时资源加载方式
            var loadedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(path);
            if (loadedRoot == null)
            {
                Debug.LogError($"Failed to load RootNode from {path}");
                return;
            }
            
            ClearGraph();
            rootNode = loadedRoot;
            rootEditor = CreateNodeEditorOnLoad(rootNode) as RootNodeEditor;
            if (rootNode.ChildNode == null)
                return;
            RecursivelyReadChildNodes(rootNode);
        }
        
        public void Save()
        {
            if (rootEditor == null)
            {
                MyDebug.LogError("No RootNodeEditor ... cannot save the tree.");
                return;
            }

            var savedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(path);
            if (savedRoot == null || !EditorUtility.IsPersistent(rootNode))
            {
                if(AssetDatabase.LoadAssetAtPath<RootNode>(path))
                    AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(rootNode, path);
                EditorUtility.SetDirty(rootNode);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path).ForEach(ass =>
                {
                    if (ass == rootNode)
                        return;
                    AssetDatabase.RemoveObjectFromAsset(ass);
                });
            }
          
            
            rootEditor.OnSave();
            EditorUtility.SetDirty(rootNode);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}