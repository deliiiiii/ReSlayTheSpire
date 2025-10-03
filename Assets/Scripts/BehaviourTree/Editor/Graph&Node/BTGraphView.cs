using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UniRx;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BehaviourTree
{
    public class BTGraphView : GraphView
    {
        GraphData graphData;
        readonly string prePath = "Assets/DataTree";
        string fileName => $"{graphData?.name ?? "null"}.asset";
        string path => $"{prePath}/{fileName}";
        IEnumerable<NodeEditorBase> nodeEditors => nodes.OfType<NodeEditorBase>();
        
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
        public void DrawNewNodeEditor(Type nodeConcreteType)
        {
            // // TODO 多余的Cast
            DrawNodeEditorByData(ScriptableObject.CreateInstance(nodeConcreteType) as BTNodeData, isDefault: true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeData">如ActionNodeDebug, 具体Node类的实例</param>
        /// <param name="isDefault">nodeConcrete是否是默认值</param>
        // TODO 多余的Cast
        void DrawNodeEditorByData<T>(T nodeData, bool isDefault) where T : BTNodeData
        {
            // 不允许创建多个RootNodeEditor
            // if (rootEditor != null && nodeData.GetType() == typeof(RootNode))
            //     return rootEditor as NodeEditor<T>;
            
            // 利用反射调用构造函数, 参数列表恰好是{T}
            var ins = typeof(BTNodeEditor<>).MakeGenericType(nodeData.GetGeneralType())
                .GetConstructor(new[] { nodeData.GetGeneralType(), typeof(bool) })?
                .Invoke(new object[] { nodeData, isDefault }) as NodeEditorBase;
            ins!.OnTypeChanged += _ => DelayOnGraphViewChanged(default);
            nodeData.OnDeserializeEnd();
            ins.ConnectEdges();
            
            AddElement(ins);
        }
        
        GraphViewChange DelayOnGraphViewChanged(GraphViewChange graphViewChange)
        {
            Observable.NextFrame().Subscribe(_ =>
            {
                MyDebug.LogWarning($"OnGraphViewChanged");
                int id = 0;
                nodeEditors.ForEach(nodeEditor =>
                {
                    nodeEditor.OnRefreshTree();
                    nodeEditor.NodeData.NodeID = id++;
                });
                Save();
            });
            return graphViewChange;
        }
        
        public void Load(GraphData fGraphData)
        {
            graphData = fGraphData;
            nodes.ForEach(RemoveElement);
            // TODO 多余的Cast
            graphData.NodeDataList.Cast<BTNodeData>().ForEach(node => DrawNodeEditorByData(node, isDefault: false));
        }
        
        void Save()
        {
            IEnumerable<NodeData> nodeBases = nodeEditors
                .Select(nodeEditor => nodeEditor.NodeData);
            if (AssetDatabase.LoadAssetAtPath<GraphData>(path))
            {
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                    .Where(ass => !nodeBases.Contains(ass))
                    .ForEach(AssetDatabase.RemoveObjectFromAsset);
            }
            else
            {
                AssetDatabase.CreateAsset(graphData, path);
            }
            EditorUtility.SetDirty(graphData);
            nodeEditors.ForEach(nodeEditor =>
            {
                AssetDataBaseExt.SafeAddSubAsset(nodeEditor.NodeData, graphData);
            });
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static List<Object> GetAllSubAssets(ScriptableObject target)
        {
            var ret = new List<Object>();
            if (target == null) 
                return ret;
            string assetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(assetPath)) 
                return ret;
            ret.AddRange(AssetDatabase.LoadAllAssetsAtPath(assetPath));
            return ret;
        }
    }
}