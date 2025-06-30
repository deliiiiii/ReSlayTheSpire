using System;
using System.Collections.Generic;
using System.Linq;
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
            CreateDropDownField();
            CreateOutputPort();
        }
        
        void CreateDropDownField()
        {
            typeField = new DropdownField(rtTypeList.Select(x => x.Name).ToList(), rtTypeList[0].Name);
            title = rtTypeList[0].Name;
            NodeBase = Activator.CreateInstance(rtTypeList[0]) as T;
                
            typeField.RegisterValueChangedCallback(choice =>
            {
                title = choice.newValue;
                NodeBase = Activator.CreateInstance(rtTypeList.First(x => x.Name == choice.newValue)) as T;
            });
            extensionContainer.Add(typeField);
        }

        void CreateOutputPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Parent ↑";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
        public void AddInEditorChildren()
        {
            //遍历节点的每个输出端口
            foreach (var outputPort in outputContainer.Q<Port>().connections)
            {
                //获取连接的输出端口对应的节点
                if (outputPort.input.node is not NodeBaseEditor<NodeBase> childNode)
                    continue;
                NodeBase.AddChild(childNode.NodeBase);
                //递归添加子节点
                childNode.AddInEditorChildren();
            }
        }
    }

    public static class DropDownFieldDataCache
    {
        //缓存每个NodeBaseEditor的DropDownFieldData
        //<ActionNodeEditor<>, [ActionNodeDebug...]>
        public static readonly Dictionary<Type, List<Type>> DDDic = new();
    }
}