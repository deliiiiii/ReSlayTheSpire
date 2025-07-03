using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
        public int? InEdgesCount { get; }
        public int? OutEdgesCount { get; }
        
        public IEnumerable<Edge> InEdges { get; }
        public IEnumerable<Edge> OutEdges { get; }
        public event Action<Type> OnNodeBaseChanged;
        void OnConstructTree();
        void OnSave();
    }
    
    

    
    
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        [ShowInInspector]
        public T NodeBase { get; protected set; }
        public int? InEdgesCount => InEdges.Count();
        public int? OutEdgesCount => OutEdges.Count();
        public IEnumerable<Edge> InEdges => inputContainer.Query<Port>().ToList()?.SelectMany(x => x.connections) ?? Enumerable.Empty<Edge>();
        public IEnumerable<Edge> OutEdges => outputContainer.Query<Port>().ToList()?.SelectMany(x => x.connections) ?? Enumerable.Empty<Edge>();
        public event Action<Type> OnNodeBaseChanged;
        
        protected DropdownField typeField;
        protected HashSet<VisualElement> fieldElementSet = new();
        
        
        /// <summary>
        /// 最终是实例类ActionNodeEditor在调用
        /// [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        List<Type> rtTypeList => DropDownFieldDataCache.DDDic[GetType()];
        public NodeBaseEditor()
        {
            if (rtTypeList.Count == 0)
            {
                MyDebug.LogError($"No {GetType().Name} types found in the assembly.");
                return;
            }
            DrawPort();
            DrawTypeField();
            OnNodeBaseChanged += SetNodeBase;
            
            OnNodeBaseChanged!.Invoke(rtTypeList[0]);
        }



        protected abstract void DrawPort();
        public abstract void OnConstructTree();
        public abstract void OnSave();
        
        protected virtual void DrawNodeField()
        {
            foreach (var field in fieldElementSet)
            {
                extensionContainer.Remove(field);
            }
            fieldElementSet.Clear();

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
        
        
        void DrawTypeField()
        {
            typeField = new DropdownField(rtTypeList.Select(x => x.Name).ToList(), rtTypeList[0].Name);
            typeField.RegisterValueChangedCallback(evt =>
            {
                OnNodeBaseChanged?.Invoke(rtTypeList.First(x => x.Name == evt.newValue));
            });
            extensionContainer.Add(typeField);
        }
        
        void SetNodeBase(Type nodeType)
        {
            if (nodeType.IsGenericType)
            {
                nodeType = nodeType.MakeGenericType(typeof(int));
            }

            // NodeBase = Activator.CreateInstance(nodeType) as T;
            NodeBase = ScriptableObject.CreateInstance(nodeType) as T;
            title = NodeBase.NodeName = nodeType.Name;
            DrawNodeField();
        }
    }

    public static class DropDownFieldDataCache
    {
        /// <summary>
        /// 缓存每个NodeBaseEditor的DropDownFieldData
        /// ActionNodeEditor, [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        public static readonly Dictionary<Type, List<Type>> DDDic = new();
    }
}