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
    [CustomEditor(typeof(RootNode))]
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
        
        
        string s1 = "DataTree";
        string s2 = nameof(RootNode);
        string path => $"Assets/{s1}/{s2}.asset";

        
        static RootNodeEditor rootEditor;
        public static RootNode rootNode;
        static RootNode rtNootNode
        {
            get => TreeTest.Root;
            set => TreeTest.Root = value;
        }
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
        
        public INodeBaseEditor<NodeBase> DrawNodeEditor(Type nodeEditorType, NodeBase nodeconcrete = null)
        {
            // 利用反射调用构造函数, 参数列表恰好是T
            var ins = nodeEditorType
                .GetConstructor(nodeEditorType.BaseType!.GetGenericArguments())
                ?.Invoke(new object[]{nodeconcrete})
                as INodeBaseEditor<NodeBase>;
            ins.OnNodeEditorChanged += _ => OnGraphViewChanged(default);
            AddElement(ins as Node);
            return ins;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeConcrete"> 如ActionNodeDebug，具体Node类</param>
        INodeBaseEditor<NodeBase> DrawNodeEditorWithConcrete<T>(T nodeConcrete) where T : NodeBase
        {
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
            Observable.NextFrame().Subscribe(_ =>
            {
                RefreshTree();
                Save();
            });
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
        
        public void Load()
        {
            //TODO 运行时资源加载方式
            var loadedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(path);
            if (loadedRoot == null)
            {
                Debug.LogError($"Failed to load RootNode from {path}");
                return;
            }
            nodes.ForEach(RemoveElement);
            rootNode = loadedRoot;
            rootEditor = DrawNodeEditorWithConcrete(rootNode) as RootNodeEditor;
            CreateChildNodeEditors(rootEditor, rootNode);
        }
        
        public void Save()
        {
            // if (rootEditor == null)
            // {
            //     MyDebug.LogError("No RootNodeEditor ... cannot save the tree.");
            //     return;
            // }

            if (AssetDatabase.LoadAssetAtPath<RootNode>(path))
            {
                // // 说明已经有的存档是上次存的，和现在的图没关系，应该删除
                // if (!EditorUtility.IsPersistent(rootNode))
                // {
                //     AssetDatabase.DeleteAsset(path);
                //     AssetDatabase.CreateAsset(rootNode, path);
                // }
                // else
                // {
                    AssetDatabase.LoadAllAssetRepresentationsAtPath(path).ForEach(ass =>
                    {
                        if (ass == null || ass == rootNode)
                            return;
                        AssetDatabase.RemoveObjectFromAsset(ass);
                    });
                // }
            }
            
            EditorUtility.SetDirty(rootNode);
            rootEditor.OnSave();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            rtNootNode = rootEditor.NodeBase;
        }
    }
}