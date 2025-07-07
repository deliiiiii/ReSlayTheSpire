using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(port => 
                    port.direction != startPort.direction 
                    && port.portType.IsAssignableFrom(startPort.portType))
                .ToList();
        }

        string path => $"Assets/DataTree/{rootNode?.name ?? "null"}.asset";
        string tempPath => $"Assets/DataTree/{rootNode?.name ?? "null"}Temp.asset";
        INodeBaseEditor<NodeBase> rootEditor;
        NodeBase rootNode;
        
        
        public BTGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = OnGraphViewChanged;
            this.StretchToParentSize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcreteType">如ActionNodeDebug，具体Node类</param>
        /// <returns></returns>
        public void DrawNodeEditorWithType(Type nodeConcreteType)
        {
            DrawNodeEditorWithIns(ScriptableObject.CreateInstance(nodeConcreteType) as NodeBase, true);
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcrete">如ActionNodeDebug，具体Node类</param>
        /// <param name="isDefault">nodeConcrete是否是默认值</param>
        INodeBaseEditor<T> DrawNodeEditorWithIns<T>(T nodeConcrete, bool isDefault) where T : NodeBase
        {
            // 不允许创建多个RootNodeEditor
            if (rootEditor != null && nodeConcrete?.GetType() == typeof(RootNode))
                return rootEditor as INodeBaseEditor<T>;
            
            // 利用反射调用构造函数, 参数列表恰好是{T}
            var ins = typeof(NodeBaseEditor<T>)
                    .GetConstructor(new []{typeof(T), typeof(bool)})
                    ?.Invoke(new object[]{nodeConcrete, isDefault})
                    as INodeBaseEditor<T>;
            ins.OnTypeChanged += _ => OnGraphViewChanged(default);
            
            var node = ins as Node;
            AddElement(node);
            return ins;
        }
        
        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            MyDebug.LogWarning($"OnGraphViewChanged");
            nodes.ForEach(node =>
            {
                if (node is not INodeBaseEditor<NodeBase> nodeEditor)
                    return;
                nodeEditor.NodeBase.ClearChildren();
            });
            RefreshTreeAndSave();
            return graphViewChange;
        }
        
        /// <summary>
        ///  更新节点连接状态。。。
        /// </summary>
        void RefreshTreeAndSave()
        {
            rootEditor = nodes.OfType<INodeBaseEditor<NodeBase>>().FirstOrDefault(node => node.NodeBase is RootNode);
            if (rootEditor == null)
            {
                MyDebug.Log("No ROOT node found in the graph, will NOT save the graph.");
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
        
        bool rootNodeDeleted = false;
        void Save()
        {
            var nodeBaseEditors = nodes
                .OfType<INodeBaseEditor<NodeBase>>().ToList();
            IEnumerable<NodeBase> nodeBases = nodeBaseEditors
                .Select(nodeEditor => nodeEditor.NodeBase)
                .ToList();
            if (AssetDatabase.LoadAssetAtPath<RootNode>(path))
            {
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                    .Where(ass => !nodeBases.Contains(ass))
                    .ForEach(AssetDatabase.RemoveObjectFromAsset);

                if (!EditorUtility.IsPersistent(rootNode))
                {
                    rootNodeDeleted = true;
                    AssetDatabase.CreateAsset(rootNode, tempPath);
                }
            }
            else
            {
                AssetDatabase.CreateAsset(rootNode, path);
            }
            EditorUtility.SetDirty(rootNode);
            nodeBaseEditors.ForEach(nodeEditor =>
            {
                AssetDataBaseExtension.SafeAddSubAsset(nodeEditor.NodeBase, rootNode);
            });
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}