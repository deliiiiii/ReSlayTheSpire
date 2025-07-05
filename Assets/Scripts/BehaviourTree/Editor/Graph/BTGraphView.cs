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
        string s1 = "DataTree";
        string s2 = nameof(RootNode);
        string path => $"Assets/{s1}/{s2}.asset";
        RootNodeEditor rootEditor;

        static RootNode rtNootNode
        {
            get => TreeTest.Root;
            set => TreeTest.Root = value;
        }

        static RootNode tempRootnode;
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

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(port => 
                    port.direction != startPort.direction 
                    && port.portType.IsAssignableFrom(startPort.portType))
                .ToList();
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
        INodeBaseEditor<NodeBase> CreateNodeEditorOnLoad<T>(T nodeConcrete) where T : NodeBase
        {
            var nodeConcreteType = nodeConcrete.GetType();
            var nodeEditorType = TypeCache.GetEditorByConcreteSubType(nodeConcreteType);
            var ret = DrawNodeEditor(nodeEditorType, nodeConcrete);
            AddElement(ret as Node);
            return ret;
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
            tempRootnode = rootEditor.NodeBase;
        }

        void ClearGraph()
        {
            nodes.ForEach(RemoveElement);
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
            tempRootnode = rtNootNode = loadedRoot;
            rootEditor = CreateNodeEditorOnLoad(rtNootNode) as RootNodeEditor;
            RecursivelyReadChildNodes(rtNootNode);
        }
        
        public void Save()
        {
            if (rootEditor == null)
            {
                MyDebug.LogError("No RootNodeEditor ... cannot save the tree.");
                return;
            }

            rtNootNode = tempRootnode;
            var savedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(path);
            if (savedRoot == null || !EditorUtility.IsPersistent(rtNootNode))
            {
                if(AssetDatabase.LoadAssetAtPath<RootNode>(path))
                    AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(rtNootNode, path);
                EditorUtility.SetDirty(rtNootNode);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path).ForEach(ass =>
                {
                    if (ass == null || ass == rtNootNode)
                        return;
                    AssetDatabase.RemoveObjectFromAsset(ass);
                });
            }
          
            
            rootEditor.OnSave();
            EditorUtility.SetDirty(rtNootNode);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
    }
}