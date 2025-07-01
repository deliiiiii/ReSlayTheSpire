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
        public NodeBase NodeBase { get; }
        void AddInEditorChildren();
    }
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public NodeBase NodeBase { get; protected set; }
        protected Port inputPort;
        protected Port outputPort;
        protected DropdownField typeField;
        /// <summary>
        /// 最终是实例类xxxNodeEditor在调用
        /// [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        List<Type> rtTypeList => DropDownFieldDataCache.DDDic[GetType().BaseType!.GetGenericTypeDefinition()];
        public NodeBaseEditor()
        {
            if (rtTypeList.Count == 0)
            {
                MyDebug.LogError($"No {GetType().Name} types found in the assembly.");
                return;
            }
            DrawNodeEditor();
        }


        protected HashSet<VisualElement> fieldElementSet = new();
        protected virtual void DrawNodeField()
        {
            // Clear existing fields except the type dropdown
            // var fieldsToRemove = extensionContainer.Children().Where(c => c != typeField).ToList();
            foreach (var field in fieldElementSet)
            {
                extensionContainer.Remove(field);
            }

            var fieldInfos = NodeBase.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<DrawnFieldAttribute>() != null);

            foreach (var fieldInfo in fieldInfos)
            {
                VisualElement fieldElement = null;

                if (fieldInfo.FieldType == typeof(string))
                {
                    var textField = new TextField(fieldInfo.Name);
                    textField.value = (string)fieldInfo.GetValue(NodeBase);
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        fieldInfo.SetValue(NodeBase, evt.newValue);
                    });
                    fieldElement = textField;
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    var intField = new IntegerField(fieldInfo.Name);
                    intField.value = (int)fieldInfo.GetValue(NodeBase);
                    intField.RegisterValueChangedCallback(evt =>
                    {
                        fieldInfo.SetValue(NodeBase, evt.newValue);
                    });
                    fieldElement = intField;
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    var floatField = new FloatField(fieldInfo.Name);
                    floatField.value = (float)fieldInfo.GetValue(NodeBase);
                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        fieldInfo.SetValue(NodeBase, evt.newValue);
                    });
                    fieldElement = floatField;
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    var boolField = new Toggle(fieldInfo.Name);
                    boolField.value = (bool)fieldInfo.GetValue(NodeBase);
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        fieldInfo.SetValue(NodeBase, evt.newValue);
                    });
                    fieldElement = boolField;
                }

                if (fieldElement == null)
                    continue;
                extensionContainer.Add(fieldElement);
                fieldElementSet.Add(fieldElement);
            }
        }
        
        void DrawNodeEditor()
        {
            DrawInputPort();
            DrawTypeField();
            SetNodeBase(rtTypeList[0]);
        }
        
        void DrawTypeField()
        {
            typeField = new DropdownField(rtTypeList.Select(x => x.Name).ToList(), rtTypeList[0].Name);
            typeField.RegisterValueChangedCallback(evt =>
            {
                SetNodeBase(rtTypeList.First(x => x.Name == evt.newValue));
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
            if (nodeType.IsGenericType)
            {
                nodeType = nodeType.MakeGenericType(typeof(int));
            }

            NodeBase = Activator.CreateInstance(nodeType) as NodeBase;
            title = NodeBase.NodeName = nodeType.Name;
            // DrawNodeField();
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
        /// ActionNodeEditor《》, [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        public static readonly Dictionary<Type, List<Type>> DDDic = new();
    }
}