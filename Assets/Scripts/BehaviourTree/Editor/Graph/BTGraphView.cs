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

        RootNode rootNode
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
            rootEditor = nodes.FirstOrDefault(node => node is RootNodeEditor) as RootNodeEditor;
            if (rootEditor == null)
            {
                MyDebug.Log("No ROOT node found in the graph.");
                return;
            }
            rootEditor.OnConstructTree();
            rootNode = rootEditor.NodeBase;
        }

        public RootNode Load()
        {
            //TODO 运行时资源加载方式
            var loadedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(path);
            if (loadedRoot != null)
            {
                rootNode = loadedRoot;
                return loadedRoot;
            }
            Debug.LogError($"Failed to load RootNode from {path}");
            return null;
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