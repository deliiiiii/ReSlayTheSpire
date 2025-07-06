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
        RootNodeEditor rootEditor;
        RootNode rootNode;
        static void InitDropDownDicCache()
        {
            var baseType = typeof(NodeBaseEditor<>);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            nodeTypes
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && !type.IsAbstract
                    && (type.BaseType?.IsAbstract ?? false)
                )
                // nodeEditorType: ActionNodeEditor or CompositeNodeEditor or DecoratorNodeEditor ...
                .ForEach(nodeEditorType =>
                {
                    
                    TypeCache.EditorToSubNodeDic[nodeEditorType] = new List<Type>();
                    var tBaseType = nodeEditorType.BaseType!.GetGenericArguments()[0];
                    var tSubTypes = tBaseType.SubType();
                    if (!tSubTypes.Any())
                    {
                        TypeCache.EditorToSubNodeDic[nodeEditorType].Add(tBaseType);
                    }
                    else
                    {
                        tSubTypes.ForEach(tSubType =>
                        {
                            TypeCache.EditorToSubNodeDic[nodeEditorType].Add(tSubType);
                        });
                    }
                });
        }
        
        
        public BTGraphView()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = OnGraphViewChanged;
            this.StretchToParentSize();
            InitDropDownDicCache();
        }
        
        public INodeBaseEditor<NodeBase> DrawNodeEditor(Type nodeEditorType, NodeBase nodeConcrete = null)
        {
            // 不允许创建多个RootNodeEditor
            if (rootEditor != null && nodeEditorType == typeof(RootNodeEditor))
                return rootEditor;
            
            // 利用反射调用构造函数, 参数列表恰好是{T}
            var ins = nodeEditorType
                .GetConstructor(nodeEditorType.BaseType!.GetGenericArguments())
                ?.Invoke(new object[]{nodeConcrete})
                as INodeBaseEditor<NodeBase>;
            ins.OnTypeChanged += _ => OnGraphViewChanged(default);
            
            var node = ins as Node;
            AddElement(node);
            return ins;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcrete"> 如ActionNodeDebug，具体Node类</param>
        INodeBaseEditor<NodeBase> DrawNodeEditorWithConcrete<T>(T nodeConcrete) where T : NodeBase
        {
            if (nodeConcrete == null)
                return null;
            var nodeConcreteType = nodeConcrete.GetType();
            var nodeEditorType = TypeCache.GetEditorByConcreteSubType(nodeConcreteType);
            return DrawNodeEditor(nodeEditorType, nodeConcrete);
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
            RefreshTree();
            Save();
            return graphViewChange;
        }
        
        /// <summary>
        ///  更新节点连接状态。。。
        /// </summary>
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

        void CreateChildNodeEditors(INodeBaseEditor<NodeBase> parentNodeEditor, NodeBase parentNodeBase)
        {
            if (parentNodeBase is RootNode parentNode)
            {
                var childNode = parentNode.ChildNode;
                var childNodeEditor = DrawNodeEditorWithConcrete(childNode) as IACDNodeEditor<ACDNode>;
                var rootNodeEditor = parentNodeEditor as RootNodeEditor;
                AddElement(rootNodeEditor.ConnectChildNodeEditor(childNodeEditor));
                CreateChildNodeEditors(childNodeEditor, childNode);
                return;
            }
            if (parentNodeBase is ACDNode acdNode)
            {
                var acdNodeEditor = parentNodeEditor as IACDNodeEditor<ACDNode>;
                if (acdNode.GuardNode != null)
                {
                    var guardEditor = DrawNodeEditorWithConcrete(acdNode.GuardNode) as GuardNodeEditor;
                    AddElement(acdNodeEditor.ConnectGuardNodeEditor(guardEditor));
                }
                acdNode.ChildList?.ForEach(childNode =>
                {
                    var childNodeEditor = DrawNodeEditorWithConcrete(childNode) as IACDNodeEditor<ACDNode>;
                    AddElement(acdNodeEditor.ConnectChildNodeEditor(childNodeEditor));
                    CreateChildNodeEditors(childNodeEditor, childNode);
                });
            }
        }
        public void Load(RootNode loadedRootNode)
        {
            nodes.ForEach(RemoveElement);
            rootNode = loadedRootNode;
            rootEditor = DrawNodeEditorWithConcrete(rootNode) as RootNodeEditor;
            CreateChildNodeEditors(rootEditor, rootNode);
        }
        
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
                    .Where(ass => !nodeBases.Contains(ass as NodeBase))
                    .ForEach(AssetDatabase.RemoveObjectFromAsset);
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