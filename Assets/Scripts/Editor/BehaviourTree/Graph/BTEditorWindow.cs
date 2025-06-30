using System;
using System.Collections.Generic;
using System.Reflection;
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
            view = new BTGraphView()
            {
                name = "GGG??",
            };
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
            // var btn = new Button(() => view.CreateNode<TestNode>()) {text = nameof(BTGraphView.CreateNode) + "<" + nameof(TestNode) + ">"};
            // toolbar.Add(btn);
            rootVisualElement.Add(toolbar);
        }

        IEnumerable<Button> CollectButtons()
        {
            var ret = new List<Button>();
            //利用反射获取所有的Button，继承于Node的非抽象类
            var nodeType = typeof(Node);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in nodeTypes)
            {
                if (!nodeType.IsAssignableFrom(type) || type.IsAbstract)
                    continue;
                var btn = new Button(() => view.CreateNode(type)) {text = nameof(BTGraphView.CreateNode) + "<" + type.Name + ">"};
                ret.Add(btn);
            }
            return ret;
        }
        
        void DestructGraph()
        {
            rootVisualElement.Remove(view);
        }
        
    }
}