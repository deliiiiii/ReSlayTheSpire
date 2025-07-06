using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
        public int? InEdgesCount => InEdges.Count();
        public int? OutEdgesCount => OutEdges.Count();
        
        public IEnumerable<Edge> InEdges { get; }
        public IEnumerable<Edge> OutEdges { get; }
        Rect GetRect();
        public event Action<Type> OnTypeChanged;
        /// 当树变化时调用，主要是从头开始构建NodeBase的连接关系
        void OnRefreshTree();
    }
    
    
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    { 
        PropertyTree _propertyTree;
        public T NodeBase { get; set; }

        public IEnumerable<Edge> InEdges => inputContainer.Query<Port>().ToList()?.SelectMany(x => x.connections) ?? Enumerable.Empty<Edge>();
        public IEnumerable<Edge> OutEdges => outputContainer.Query<Port>().ToList()?.SelectMany(x => x.connections) ?? Enumerable.Empty<Edge>();
        public Rect GetRect() => GetPosition();
        public event Action<Type> OnTypeChanged;
        
        DropdownField typeField;
        HashSet<VisualElement> fieldElementSet = new();
        


        /// <summary>
        /// 最终是实例类ActionNodeEditor在调用
        /// [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        List<Type> rtTypeList => TypeCache.EditorToSubNodeDic[GetType()];

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
            DrawPort();
            DrawTypeField();
            DrawNodeField();
            
            RefreshPorts();
            expanded = true;
            RefreshExpandedState();
        }
        
        protected abstract void DrawPort();

        public virtual void OnRefreshTree()
        {
            NodeBase.RectInGraph = GetPosition();
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
            
            
            _propertyTree = PropertyTree.Create(NodeBase);
            var container = new IMGUIContainer(() =>
            {
                _propertyTree.Draw();
            });
            extensionContainer.Add(container);

            return;
            
            
            
            
            // foreach (var field in fieldElementSet)
            // {
            //     extensionContainer.Remove(field);
            // }
            // fieldElementSet.Clear();
            //
            // var fieldInfos = NodeBase.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
            //     .Where(f => f.GetCustomAttribute<DrawnFieldAttribute>() != null);
            // foreach (var fieldInfo in fieldInfos)
            // {
            //     VisualElement fieldElement = null;
            //
            //     if (fieldInfo.FieldType == typeof(string))
            //     {
            //         var textField = new TextField(fieldInfo.Name);
            //         textField.value = (string)fieldInfo.GetValue(NodeBase);
            //         textField.RegisterValueChangedCallback(evt =>
            //         {
            //             fieldInfo.SetValue(NodeBase, evt.newValue);
            //         });
            //         fieldElement = textField;
            //     }
            //     else if (fieldInfo.FieldType == typeof(int))
            //     {
            //         var intField = new IntegerField(fieldInfo.Name);
            //         intField.value = (int)fieldInfo.GetValue(NodeBase);
            //         intField.RegisterValueChangedCallback(evt =>
            //         {
            //             fieldInfo.SetValue(NodeBase, evt.newValue);
            //         });
            //         fieldElement = intField;
            //     }
            //     else if (fieldInfo.FieldType == typeof(float))
            //     {
            //         var floatField = new FloatField(fieldInfo.Name);
            //         floatField.value = (float)fieldInfo.GetValue(NodeBase);
            //         floatField.RegisterValueChangedCallback(evt =>
            //         {
            //             fieldInfo.SetValue(NodeBase, evt.newValue);
            //         });
            //         fieldElement = floatField;
            //     }
            //     else if (fieldInfo.FieldType == typeof(bool))
            //     {
            //         var boolField = new Toggle(fieldInfo.Name);
            //         boolField.value = (bool)fieldInfo.GetValue(NodeBase);
            //         boolField.RegisterValueChangedCallback(evt =>
            //         {
            //             fieldInfo.SetValue(NodeBase, evt.newValue);
            //         });
            //         fieldElement = boolField;
            //     }
            //
            //     if (fieldElement == null)
            //         continue;
            //     extensionContainer.Add(fieldElement);
            //     fieldElementSet.Add(fieldElement);
            // }
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
    }

    
}