using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BTEditorWindow : EditorWindow
    {
        static BTEditorWindow()
        {
            EditorApplication.quitting += ClearWindowFunc;
            ClearWindowFunc();
        }
        static readonly Dictionary<int, BTEditorWindow> windowDic = new();
        static BTGraphView curView;
        static BTEditorWindow curWindow;
        static RootNode curRootNode;
        
        void OnEnable()
        {
            InitView();
            InitToolbar();
        }
        void OnDisable()
        {
            var kvp = windowDic.FirstOrDefault(kvp => kvp.Value == this);
            windowDic.Remove(kvp.Key);
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is not RootNode node)
                return false;
            curRootNode = node;
            InitWindow();
            return true;
        }
        
        static void InitWindow()
        {
            var curId = curRootNode.GetInstanceID();
            if (windowDic.TryGetValue(curId, out var value))
            {
                curWindow = value;
                curWindow.Focus();
                return;
            }
            curWindow = CreateWindow<BTEditorWindow>();
            curWindow.titleContent = new GUIContent(curRootNode.name);
            curWindow.minSize = new Vector2(600, 400);
            curWindow.Show();
            windowDic.Add(curId, curWindow);
            
        }

        void InitView()
        {
            curView = new BTGraphView();
            curView.Load(curRootNode);
            rootVisualElement.Add(curView);
        }

        void InitToolbar()
        {
            var toolbar = new Toolbar();
            foreach (var btn in CollectButtons(curView))
            {
                toolbar.Add(btn);
            }
            rootVisualElement.Add(toolbar);
        }
        
        
        static IEnumerable<Button> CollectButtons(BTGraphView fView)
        {
            List<Button> ret = new();
            var baseType = typeof(NodeBaseEditor<>);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            nodeTypes
                // ActionNodeEditor, CompositeNodeEditor, DecoratorNodeEditor, RootNodeEditor, GuardNodeEditor
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && !type.IsAbstract
                    && (type.BaseType?.IsAbstract ?? false)
                    ) 
                .ForEach(nodeEditorConcreteType =>
                {
                    // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                    ret.Add(new Button(() => fView.DrawNodeEditor(nodeEditorConcreteType))
                    {
                        text = nodeEditorConcreteType.Name,
                        
                        style =
                        {
                            width = 200,
                            height = 30,
                            marginLeft = ret.Count == 0 ? 10: 0
                        }
                    });
                });
            return ret;
        }
        
        /// <summary>
        /// 关闭windowDic中所有的窗口
        /// </summary>
        static void ClearWindowFunc()
        {
            foreach (var kvp in windowDic)
            {
                kvp.Value.Close();
            }
            windowDic.Clear();
        }
    }
 
    
}