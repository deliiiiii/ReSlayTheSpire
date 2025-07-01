using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
        void AddInEditorChildren();
        public event Action<Type> OnChangeTypeEvent;
        public void SetNodeBase(Type nodeType);
    }
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public T NodeBase { get; protected set; }
        protected Port inputPort;
        protected Port outputPort;
        protected DropdownField typeField;
        //最终是实例类xxxNodeEditor在调用
        List<Type> rtTypeList => DropDownFieldDataCache.DDDic[GetType().BaseType!.GetGenericTypeDefinition()];
        
        /// <summary>
        /// 如ActionNodeDebug
        /// </summary>
        Type firstTType => rtTypeList[0].GetGenericArguments()[0];
        public event Action<Type> OnChangeTypeEvent;
        public NodeBaseEditor()
        {
            if (rtTypeList.Count == 0)
            {
                MyDebug.LogError($"No {GetType().Name} types found in the assembly.");
                return;
            }
            DrawNodeEditor();
            DrawInputPort();
        }

        protected virtual T CreateConcreteNode(Type concreteT)
        {
            return Activator.CreateInstance(concreteT) as T;
        }
        
        void DrawNodeEditor()
        {
            SetNodeBase(firstTType);
            title = NodeBase.NodeName = firstTType.Name;
            
            typeField = new DropdownField(rtTypeList.Select(x => x.GetGenericArguments()[0].Name).ToList(), firstTType.Name);
            typeField.RegisterValueChangedCallback(newNodeType =>
            {
                MyDebug.Log($" 11 {GetType().Name}");
                OnChangeTypeEvent?.Invoke(rtTypeList.First(x => x.GetGenericArguments()[0].Name == newNodeType.newValue));
            });
            extensionContainer.Add(typeField);
        }
        
        void DrawInputPort()
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Parent ↑";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);
        }
        
        public void SetNodeBase(Type nodeType)
        {
            NodeBase = CreateConcreteNode(nodeType);
        }
        
        public void AddInEditorChildren()
        {
            outputContainer.Q<Port>()?.connections.ForEach(port =>
            {
                if (port.input.node is not NodeBaseEditor<NodeBase> childNode)
                    return;
                MyDebug.Log($"{NodeBase.NodeName} AddChild {childNode.NodeBase.NodeName}");
                NodeBase.AddChild(childNode.NodeBase);
                childNode.AddInEditorChildren();
            });
        }
    }

    public static class DropDownFieldDataCache
    {
        /// <summary>
        /// 缓存每个NodeBaseEditor的DropDownFieldData
        /// ActionNodeEditor《》, [ActionNodeEditor《ActionNodeDebug》...]
        /// </summary>
        public static readonly Dictionary<Type, List<Type>> DDDic = new();
    }
}