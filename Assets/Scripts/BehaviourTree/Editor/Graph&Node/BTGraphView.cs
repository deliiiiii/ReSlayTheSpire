﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public event Action<int> OnRootNodeDeleted;
        INodeBaseEditor<RootNode> rootEditor;
        RootNode rootNode;
        
        string path => $"Assets/DataTree/{rootNode?.name ?? "null"}.asset";
        IEnumerable<INodeBaseEditor<NodeBase>> nodeEditors => 
            nodes.OfType<INodeBaseEditor<NodeBase>>().ToList();
        
        public BTGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = DelayOnGraphViewChanged;
            this.StretchToParentSize();
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(port => 
                    port.direction != startPort.direction 
                    && port.portType.IsAssignableFrom(startPort.portType))
                .ToList();
        }

        /// <param name="nodeConcreteType">如ActionNodeDebug，具体Node类型</param>
        /// <returns></returns>
        public void DrawNodeEditorWithType(Type nodeConcreteType)
        {
            DrawNodeEditorWithIns(ScriptableObject.CreateInstance(nodeConcreteType) as NodeBase, true);
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcrete">如ActionNodeDebug, 具体Node类的实例</param>
        /// <param name="isDefault">nodeConcrete是否是默认值</param>
        INodeBaseEditor<T> DrawNodeEditorWithIns<T>(T nodeConcrete, bool isDefault) where T : NodeBase
        {
            // 不允许创建多个RootNodeEditor
            if (rootEditor != null && nodeConcrete?.GetType() == typeof(RootNode))
                return rootEditor as INodeBaseEditor<T>;
            
            // 利用反射调用构造函数, 参数列表恰好是{T}
            if (typeof(NodeBaseEditor<>).MakeGenericType(nodeConcrete!.GetGeneralType())
                    .GetConstructor(new []{nodeConcrete!.GetGeneralType(), typeof(bool)})
                    ?.Invoke(new object[]{nodeConcrete, isDefault}) is not INodeBaseEditor<T> ins)
                return null;
            ins.OnTypeChanged += _ => DelayOnGraphViewChanged(default);
            
            var node = ins as Node;
            AddElement(node);
            return ins;
        }
        
        GraphViewChange DelayOnGraphViewChanged(GraphViewChange graphViewChange)
        {
            Observable.NextFrame().Subscribe(_ =>
            {
                MyDebug.LogWarning($"OnGraphViewChanged");
                nodeEditors.ForEach(node => node.NodeBase.ClearChildren());
                RefreshTreeAndSave();
            });
            return graphViewChange;
        }
        
        /// <summary>
        ///  更新节点连接状态。。。
        /// </summary>
        void RefreshTreeAndSave()
        {
            rootEditor = nodeEditors.FirstOrDefault(node => node.NodeBase is RootNode) as INodeBaseEditor<RootNode>;
            if (rootEditor == null)
            {
                MyDebug.LogError("No ROOT node found in the graph, NOT save the graph and CLOSE the window!");
                OnRootNodeDeleted?.Invoke(rootNode.GetInstanceID());
                return;
            }
            rootEditor.OnRefreshTree();
            rootNode = rootEditor.NodeBase;
            Save();
        }

        void CreateChildNodeEditors(INodeBaseEditor<NodeBase> thisNodeEditor, NodeBase thisNodeBase)
        {
            var guardNode = thisNodeBase.GuardNode;
            if (guardNode != null)
            {
                var guardEditor = DrawNodeEditorWithIns(guardNode, false);
                var ele = thisNodeEditor.ConnectGuardNodeEditor(guardEditor);
                if (ele == null)
                {
                    MyDebug.LogError($"Failed to connect guardNodeEditor {guardNode.name} for {thisNodeBase.name}, probably some port is NULL! Check the config.");
                }
                else
                {
                    AddElement(ele);
                }
                CreateChildNodeEditors(guardEditor, guardNode);
            }
            thisNodeBase.ChildList?.ForEach(childNode =>
            {
                var childNodeEditor = DrawNodeEditorWithIns(childNode, false);
                var ele = thisNodeEditor.ConnectChildNodeEditor(childNodeEditor);
                if (ele == null)
                {
                    MyDebug.LogError($"Failed to connect childNodeEditor {childNode.name} for {thisNodeBase.name}, probably some port is NULL! Check the config.");
                }
                else
                {
                    AddElement(ele);
                }
                CreateChildNodeEditors(childNodeEditor, childNode);
            });
        }
        public void Load(RootNode loadedRootNode)
        {
            nodes.ForEach(RemoveElement);
            rootNode = loadedRootNode;
            rootEditor = DrawNodeEditorWithIns(rootNode, false);
            CreateChildNodeEditors(rootEditor, rootNode);
        }
        
        void Save()
        {
            IEnumerable<NodeBase> nodeBases = nodeEditors
                .Select(nodeEditor => nodeEditor.NodeBase)
                .ToList();
            if (AssetDatabase.LoadAssetAtPath<RootNode>(path))
            {
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                    .Where(ass => !nodeBases.Contains(ass))
                    .ForEach(AssetDatabase.RemoveObjectFromAsset);
            }
            else
            {
                AssetDatabase.CreateAsset(rootNode, path);
            }
            EditorUtility.SetDirty(rootNode);
            nodeEditors.ForEach(nodeEditor =>
            {
                AssetDataBaseExt.SafeAddSubAsset(nodeEditor.NodeBase, rootNode);
            });
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}