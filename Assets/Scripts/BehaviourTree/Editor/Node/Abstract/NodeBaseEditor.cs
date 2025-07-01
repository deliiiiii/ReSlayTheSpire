using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
        void AddInEditorChildren();
    }
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public T NodeBase { get; protected set; }
        protected Port inputPort;
        protected Port outputPort;
        protected DropdownField typeField;
        //最终是实例类xxxNodeEditor在调用
        List<Type> rtTypeList => DropDownFieldDataCache.DDDic[GetType().BaseType!.GetGenericTypeDefinition()];
        
        public NodeBaseEditor()
        {
            if (rtTypeList.Count == 0)
            {
                MyDebug.LogError($"No {GetType().Name} types found in the assembly.");
                return;
            }
            CreateNodeEditor();
            CreateInputPort();
        }
        
        void CreateNodeEditor()
        {
            typeField = new DropdownField(rtTypeList.Select(x => x.GetGenericArguments()[0].Name).ToList(), rtTypeList[0].GetGenericArguments()[0].Name);
            title = rtTypeList[0].GetGenericArguments()[0].Name;
            NodeBase = Activator.CreateInstance(rtTypeList[0].GetGenericArguments()[0]) as T;
            NodeBase.NodeName = rtTypeList[0].GetGenericArguments()[0].Name;
                
            typeField.RegisterValueChangedCallback(choice =>
            {
                OnChangeType(rtTypeList.First(x => x.GetGenericArguments()[0].Name == choice.newValue));
            });
            extensionContainer.Add(typeField);
        }

        void OnChangeType(Type nodeEditorType)
        {
            // title = nodeEditorType.Name;
            // NodeBase = Activator.CreateInstance(nodeEditorType);
            // MyDebug.Log($"select: changed to {choice.newValue}");
            // MyDebug.Log($"{NodeBase == null} {NodeBase?.GetType() == null} {NodeBase?.GetType().Name}");
            //     
            // NodeBase.NodeName = choice.newValue;
        }
        void CreateInputPort()
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Parent ↑";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);
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
        //缓存每个NodeBaseEditor的DropDownFieldData
        //<ActionNodeEditor<>, [ActionNodeEditor<ActionNodeDebug>...]>
        public static readonly Dictionary<Type, List<Type>> DDDic = new();
    }
}