using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class BTEditorWindow : EditorWindow
    {
        BTGraphView view;
        [MenuItem("BTGraph/Open Graph Editor")]
        public static void OnOpen()
        {
            InitWindow();
        }
        void OnEnable()
        {
            ConstructGraph();
            ConstructToolbar();
        }
        void OnDisable()
        {
            DestructGraph();
        }

        static void InitWindow()
        {
            var window = GetWindow<BTEditorWindow>();
            window.titleContent = new GUIContent("BT Graph");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }
        
        void ConstructGraph()
        {
            view = new BTGraphView();
            view.graphViewChanged = view.OnGraphViewChanged;
            view.StretchToParentSize();
            rootVisualElement.Add(view);
        }
        
        void ConstructToolbar()
        {
            var toolbar = new Toolbar();
            foreach (var btn in CollectButtons())
            {
                toolbar.Add(btn);
            }
            rootVisualElement.Add(toolbar);
        }

        IEnumerable<Button> CollectButtons()
        {
            var ret = new List<Button>();
            // 利用反射获取所有的Button，继承于NodeBaseEditor的非抽象类
            var baseType = typeof(NodeBaseEditor<>);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            
            foreach (var type in nodeTypes)
            {
                var createdType = baseType;
                var b1 = type.InheritsFrom(baseType);
                if (!b1 || !type.IsAbstract)
                    continue;
                if(type == typeof(NodeBaseEditor<>))
                    continue;
                
                // 已知
                // ActionNodeDebugEditor : ActionNodeEditor<ActionNodeDebug>
                // ActionNodeEditor<T> where T : ActionNode
                // 怎么获取其泛型参数T的一个具体类型,如 T = ActionNodeDebug
                if (type.IsGenericType)
                {
                    // var genericType = type.GetGenericArguments()[0];
                    //拿到泛型T的一个具体类型
                    // createdType = type.MakeGenericType(genericType.BaseType.FirstSubType());
                    //!!!!! TODO
                    createdType = type.FirstSubType();
                }
                var btn = new Button(() => view.CreateNode(createdType)) {text = nameof(BTGraphView.CreateNode) + "<" + type.Name + ">"};
                ret.Add(btn);
            }
            return ret;
        }
        
        void DestructGraph()
        {
            rootVisualElement.Remove(view);
        }
        
    }

    public static class ReflectionExtensions
    {
        public static Type FirstSubType(this Type parentType)
        {
            return parentType.Assembly.GetTypes().FirstOrDefault(x => parentType.IsAssignableFrom(x) && x != parentType);

        }
        
    }
}