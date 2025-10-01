using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public abstract class NodeEditorBase : Node
    {
        protected static readonly PortToDrawConfig portToDrawConfig;
        protected static readonly Dictionary<EState, Color> tickStateColorDic = new()
        {
            { EState.Running, Color.cyan },
            { EState.Succeeded, Color.green },
            { EState.Failed, Color.red }
        };

        static NodeEditorBase()
        {
            portToDrawConfig =
                AssetDatabase.LoadAssetAtPath<PortToDrawConfig>("Assets/DataTree/PortToDrawConfig.asset");
            if (portToDrawConfig == null)
            {
                MyDebug.LogError(
                    "PortToDrawConfig not found, please create it in Assets/DataTree/PortToDrawConfig.asset");
            }
        }

        protected Dictionary<string, Port> portDic = new();

        public NodeData NodeData;
        public Port InputParentPort;
        public Port InputGuardingPort;
        public Port OutputChildPorts;
        public Port OutputGuardedPort;

        public Action<Type> OnTypeChanged;

        IEnumerable<NodeEditorBase> ChildNodeEditors =>
            portDic.Values
                .Where(p => p.direction == Direction.Output)
                .SelectMany(p => p.connections.Select(c => c.input.node))
                .OfType<NodeEditorBase>();
            
        
        [CanBeNull]
        public Edge ConnectEdges()
        {
            // TODO
            return null;
        }
        
        /// <summary>
        /// 当树变化时调用，主要是从头开始构建NodeBase的连接关系
        /// </summary>
        public void OnRefreshTree()
        {
            NodeData.Position = GetRect().position;
            NodeData.Size = GetRect().size;
            NodeData.ClearChildren();
            ChildNodeEditors?
                .OrderBy(nodeEditor => nodeEditor.NodeData.Position.x)
                .ForEach(nodeEditor =>
                {
                    MyDebug.Log($"OnRefreshTree : {NodeData.name} AddChild {nodeEditor.NodeData.name}");
                    NodeData.AddChild(nodeEditor.NodeData);
                    // nodeDAta.OnRefreshTree();
                });
            
            // NodeData.GuardNode = this.GuardingNode();
            // this.GuardingEditor()?.OnRefreshTree();
            // MyDebug.Log($"Editor : {NodeBase.name} AddGuard {guard?.name ?? "null"}");
            NodeData.OnRefreshTreeEnd();
        }

        Rect GetRect() => GetPosition();
    }
    
    // TODO 多余的Cast
    [Serializable]
    public class NodeEditor<T> : NodeEditorBase where T : BTNodeData
    {
        public new T NodeData
        {
            get => base.NodeData as T;
            set
            {
                if (base.NodeData == value)
                    return;
                base.NodeData = value;
                extensionContainer.Clear();
                DrawTypeField();
                DrawNodeField();
            }
        }

        public Rect GetRect() => GetPosition();
        
        public NodeEditor(T nodeData, bool isDefault)
        {
            NodeData = nodeData;
            OnTypeChanged += nodeType =>
            {
                NodeData = ScriptableObject.CreateInstance(nodeType) as T;
                NodeData!.name = NodeData.GetType().Name;
            };
            if (isDefault)
            {
                base.SetPosition(new Rect(600, 600, 400, 250));
            }
            else
            {
                var rect = new Rect(NodeData.Position, NodeData.Size);
                base.SetPosition(rect);
                style.width = rect.width;
                style.height = rect.height;
            }
            
            NodeData!.name = NodeData.GetType().Name;
            DrawAllPorts();
            //隐藏名字栏
            titleContainer.style.display = DisplayStyle.None;
            //大小可以调整
            capabilities |= Capabilities.Resizable;
            
            // 重置一些内置参数
            RefreshPorts();
            base.expanded = true;
            RefreshExpandedState();
        }

        void DrawAllPorts()
        { 
            portDic.Clear();
            var portToDrawData = portToDrawConfig.TypeToPortToDrawData[NodeData.GetGeneralType().Name];
            portToDrawData.ForEach(nameToPortData =>
            {
                var value = nameToPortData.Value;
                if (!value.IsValid)
                    return;
                var ins = InstantiatePort(Orientation.Vertical, value.Direction, value.Capacity,
                    TypeCache.PortTypeDic[value.PortTypeName]);
                ins.portName = value.PortName;
                if (value.Direction == Direction.Input)
                {
                    inputContainer.Add(ins);
                }
                else
                {
                    outputContainer.Add(ins);
                }
                GetType()
                    .GetField(nameToPortData.Key)
                    .SetValue(this, ins);
                
                portDic.Add(nameToPortData.Key, ins);
            });
            
        }
        
        void DrawTypeField()
        {
            var selections = TypeCache.GeneralToSelectionsDic[NodeData.GetGeneralType()];
            var typeField = new DropdownField(selections.Select(x => x.Name).ToList(), NodeData.GetType().Name);
            typeField.RegisterValueChangedCallback(evt =>
            {
                OnTypeChanged?.Invoke(selections.First(x => x.Name == evt.newValue));
            });
            extensionContainer.Add(typeField);
        }

        void DrawNodeField()
        {
            style.backgroundColor = tickStateColorDic[NodeData.State];
            NodeData.State.OnValueChangedAfter += evt =>
            {
                style.backgroundColor = tickStateColorDic[evt];
            };
            
            var nodeField = new ObjectField()
            {
                objectType = NodeData.GetType(),
                value = NodeData,
                style =
                {
                    marginRight = -200,
                },
            };
            extensionContainer.Add(nodeField);

            var detailLabel = new Label(NodeData.GetDetail())
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    color = Color.white,
                    fontSize = 15,
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            };
            detailLabel.schedule.Execute(() =>
            {
                detailLabel.text = NodeData.GetDetail();
            }).Every(10);
            extensionContainer.Add(detailLabel);
        }
    }

    
}