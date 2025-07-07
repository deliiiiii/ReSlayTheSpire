using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Config;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        T NodeBase { get; }
        Port InputParentPort { get; }
        Port InputGuardingPort { get; }
        Port OutputChildsPort { get; }
        Port OutputGuardedPort { get; }

        public event Action<Type> OnTypeChanged;

        [CanBeNull]
        public Edge ConnectChildNodeEditor(INodeBaseEditor<NodeBase> childNodeEditor)
        {
            return OutputChildsPort?.ConnectTo(childNodeEditor.InputParentPort);
        }
        [CanBeNull]
        public Edge ConnectGuardNodeEditor(INodeBaseEditor<NodeBase> guardNodeEditor)
        {
            return InputGuardingPort?.ConnectTo(guardNodeEditor.OutputGuardedPort);
        }
        void OnRefreshTree();
        Rect GetRect();
    }


    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public T NodeBase { get; private set; }
        public Port InputParentPort { get; protected set; }
        public Port InputGuardingPort { get; protected set; }
        public Port OutputChildsPort { get; protected set; }
        public Port OutputGuardedPort { get; protected set; }
        [CanBeNull]
        IEnumerable<INodeBaseEditor<NodeBase>> childsEditor =>
            OutputChildsPort?.connections?
                .Where(port => port.input.node is INodeBaseEditor<NodeBase>)
                .Select(port => port.input.node as INodeBaseEditor<NodeBase>);
        [CanBeNull]
        INodeBaseEditor<GuardNode> guardingEditor =>
            InputGuardingPort?.connections?
                .Where(port => port.output.node is GuardNodeEditor)
                .Select(port => port.output.node as GuardNodeEditor)
                .FirstOrDefault();
        [CanBeNull] GuardNode guardingNode => guardingEditor?.NodeBase;
        public event Action<Type> OnTypeChanged;

        public Rect GetRect() => GetPosition();

        DropdownField typeField;
        PropertyTree propertyTree;
        IMGUIContainer propertyTreeContainer;

        /// <summary>
        /// 最终是实例类ActionNodeEditor在调用
        /// [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        List<Type> rtTypeList => TypeCache.EditorToSubNodeDic[GetType()];

        static PortToDrawConfig portToDrawConfig;

        static NodeBaseEditor()
        {
            portToDrawConfig =
                AssetDatabase.LoadAssetAtPath<PortToDrawConfig>("Assets/DataTree/PortToDrawConfig.asset");
            if (portToDrawConfig == null)
            {
                MyDebug.LogError("PortToDrawConfig not found, please create it in Assets/DataTree/PortToDrawConfig.asset");
            }
        }

        public NodeBaseEditor(T nodeBase)
        {
            if (rtTypeList.Count == 0)
            {
                MyDebug.LogError($"No {GetType().Name} types found in the assembly.");
                return;
            }
            // MyDebug.Log($"{GetType().Name} types found in the assembly.");

            if (nodeBase == null)
            {
                CreateNodeBase(rtTypeList[0]);
                SetPosition(new Rect(100, 100, 200, 150));
            }
            else
            {
                NodeBase = nodeBase;
                SetPosition(NodeBase.RectInGraph);
            }

            OnTypeChanged += CreateNodeBase;
            OnTypeChanged += _ => DrawNodeField();

            //隐藏名字栏
            titleContainer.style.display = DisplayStyle.None;
            DrawAllPorts();
            DrawTypeField();
            DrawNodeField();

            RefreshPorts();
            expanded = true;
            RefreshExpandedState();
        }

        /// <summary>
        /// 当树变化时调用，主要是从头开始构建NodeBase的连接关系
        /// </summary>
        public void OnRefreshTree()
        {
            NodeBase.RectInGraph = GetPosition();
            NodeBase.ClearChildren();
            childsEditor?
                .OrderBy(editor => editor.GetRect().x)
                .ForEach(childEditor =>
                {
                    // MyDebug.Log($"Editor : {NodeBase.name} AddChild {childEditor.NodeBase.name}");
                    NodeBase.AddChild(childEditor.NodeBase);
                    childEditor.OnRefreshTree();
                });
            
            NodeBase.GuardNode = guardingNode;
            guardingEditor?.OnRefreshTree();
            // MyDebug.Log($"Editor : {NodeBase.name} AddGuard {guard?.name ?? "null"}");
        }
        
        void CreateNodeBase(Type nodeType)
        {
            if (nodeType.IsGenericType)
            {
                nodeType = nodeType.MakeGenericType(typeof(int));
            }
            NodeBase = ScriptableObject.CreateInstance(nodeType) as T;
            NodeBase.name = nodeType.Name;
        }

        void DrawAllPorts()
        {
            var portToDrawData = portToDrawConfig.TypeToPortToDrawData[NodeBase.GetNodeGeneralType().Name];
            portToDrawData.ForEach(nameToPortData =>
            {
                var value = nameToPortData.Value;
                if (!value.IsValid)
                    return;
                var ins = InstantiatePort(Orientation.Vertical, value.Direction, value.Capacity,
                    typeof(int));
                ins.portName = value.Name;
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
            typeField = new DropdownField(rtTypeList.Select(x => x.Name).ToList(), NodeBase.GetType().Name);
            typeField.RegisterValueChangedCallback(evt =>
            {
                OnTypeChanged?.Invoke(rtTypeList.First(x => x.Name == evt.newValue));
            });
            extensionContainer.Add(typeField);
        }

        readonly Dictionary<EState, Color> tickStateColorDic = new()
        {
            { EState.Running, Color.cyan },
            { EState.Succeeded, Color.green },
            { EState.Failed, Color.red }
        };

        void DrawNodeField()
        {
            style.backgroundColor = tickStateColorDic[NodeBase.State];
            NodeBase.State.OnValueChangedAfter += evt =>
            {
                style.backgroundColor = tickStateColorDic[evt];
            };

            propertyTree?.Dispose();
            if (propertyTreeContainer != null)
            {
                extensionContainer.Remove(propertyTreeContainer);
            }
            propertyTree = PropertyTree.Create(NodeBase);
            propertyTreeContainer = new IMGUIContainer(() => propertyTree.Draw());
            extensionContainer.Add(propertyTreeContainer);
        }
    }

    
}