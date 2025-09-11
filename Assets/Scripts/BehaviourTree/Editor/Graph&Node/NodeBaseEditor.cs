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
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        T NodeBase { get; }
        Port InputParentPort { get; }
        Port InputGuardingPort { get; }
        Port OutputChildPorts { get; }
        Port OutputGuardedPort { get; }

        public event Action<Type> OnTypeChanged;
        [CanBeNull]
        public Edge ConnectChildNodeEditor(INodeBaseEditor<NodeBase> childNodeEditor)
        {
            return OutputChildPorts?.ConnectTo(childNodeEditor.InputParentPort);
        }
        [CanBeNull]
        public Edge ConnectGuardNodeEditor(INodeBaseEditor<NodeBase> guardNodeEditor)
        {
            return InputGuardingPort?.ConnectTo(guardNodeEditor.OutputGuardedPort);
        }
        
        /// <summary>
        /// 当树变化时调用，主要是从头开始构建NodeBase的连接关系
        /// </summary>
        public void OnRefreshTree()
        {
            NodeBase.Position = GetRect().position;
            NodeBase.Size = GetRect().size;
            NodeBase.ClearChildren();
            this.ChildEditors()?
                .OrderBy(editor => editor.GetRect().x)
                .ForEach(childEditor =>
                {
                    // MyDebug.Log($"Editor : {NodeBase.name} AddChild {childEditor.NodeBase.name}");
                    NodeBase.AddChild(childEditor.NodeBase);
                    childEditor.OnRefreshTree();
                });
            
            NodeBase.GuardNode = this.GuardingNode();
            this.GuardingEditor()?.OnRefreshTree();
            // MyDebug.Log($"Editor : {NodeBase.name} AddGuard {guard?.name ?? "null"}");
            NodeBase.OnRefreshTreeEnd();
        }

        Node Node { get; }
        Rect GetRect() => Node.GetPosition();
    }
    
    [Serializable]
    public class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public Node Node => this;
        T nodeBase;
        public T NodeBase
        {
            get => nodeBase;
            set
            {
                if (nodeBase == value)
                    return;
                nodeBase = value;
                extensionContainer.Clear();
                DrawTypeField();
                DrawNodeField();
            }
        }
        public Port InputParentPort { get; protected set; }
        public Port InputGuardingPort { get; protected set; }
        public Port OutputChildPorts { get; protected set; }
        public Port OutputGuardedPort { get; protected set; }
        
        public event Action<Type> OnTypeChanged;

        public Rect GetRect() => GetPosition();
        DropdownField typeField;
        ObjectField nodeField;
        Label detailLabel;
        
        static readonly PortToDrawConfig portToDrawConfig;
        static readonly Dictionary<EState, Color> tickStateColorDic = new()
        {
            { EState.Running, Color.cyan },
            { EState.Succeeded, Color.green },
            { EState.Failed, Color.red }
        };

        static NodeBaseEditor()
        {
            portToDrawConfig =
                AssetDatabase.LoadAssetAtPath<PortToDrawConfig>("Assets/DataTree/PortToDrawConfig.asset");
            if (portToDrawConfig == null)
            {
                MyDebug.LogError("PortToDrawConfig not found, please create it in Assets/DataTree/PortToDrawConfig.asset");
            }
        }
        
        public NodeBaseEditor(T nodeBase, bool isDefault)
        {
            // MyDebug.Log($"NodeBaseEditor({nodeBase.GetType().Name}, {isDefault})");
            OnTypeChanged += CreateNodeBase;
            if (isDefault)
            {
                CreateNodeBase(nodeBase.GetType());
                base.SetPosition(new Rect(600, 600, 400, 250));
            }
            else
            {
                NodeBase = nodeBase;
                var rect = new Rect(NodeBase.Position, NodeBase.Size);
                base.SetPosition(rect);
                style.width = rect.width;
                style.height = rect.height;
            }
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
        
        void CreateNodeBase(Type nodeType)
        {
            NodeBase = ScriptableObject.CreateInstance(nodeType) as T;
            NodeBase.name = nodeType.Name;
        }

        void DrawAllPorts()
        {
            var portToDrawData = portToDrawConfig.TypeToPortToDrawData[NodeBase.GetGeneralType().Name];
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
                    .GetProperty(nameToPortData.Key)!
                    .SetValue(this, ins, null);
            });
        }
        
        void DrawTypeField()
        {
            var selections = TypeCache.GeneralToSelectionsDic[NodeBase.GetGeneralType()];
            typeField = new DropdownField(selections.Select(x => x.Name).ToList(), NodeBase.GetType().Name);
            typeField.RegisterValueChangedCallback(evt =>
            {
                OnTypeChanged.Invoke(selections.First(x => x.Name == evt.newValue));
            });
            extensionContainer.Add(typeField);
        }

        void DrawNodeField()
        {
            style.backgroundColor = tickStateColorDic[NodeBase.State];
            NodeBase.State.OnValueChangedAfter += evt =>
            {
                style.backgroundColor = tickStateColorDic[evt];
            };
            
            nodeField = new ObjectField
            {
                objectType = NodeBase.GetType(),
                value = NodeBase,
                style =
                {
                    marginRight = -200,
                },
            };
            extensionContainer.Add(nodeField);

            if (NodeBase is IShowDetail sd)
            {
                detailLabel = new Label(sd.GetDetail())
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
                    detailLabel.text = sd.GetDetail();
                }).Every(10);
                extensionContainer.Add(detailLabel);
            }
        }
    }

    
}