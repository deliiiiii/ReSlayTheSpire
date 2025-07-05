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
        static BTGraphView view;
        static BTEditorWindow window;
        static RootNode openedRootNode;
        [MenuItem("BTGraph/Open Graph Editor")]
        public static void OnOpen()
        {
            window = GetWindow<BTEditorWindow>();
            window.titleContent = new GUIContent("BT Graph");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is not RootNode node)
                return false; // 继续默认行为
            openedRootNode = node;
            OnOpen();
            return true; // 表示已处理，阻止默认行为
        }
        void OnEnable()
        {
            view = new BTGraphView();
            BTGraphView.rootNode = openedRootNode;
            view.Load();
            rootVisualElement.Add(view);
            ConstructToolbar();
        }
        void OnDisable()
        {
            rootVisualElement.Remove(view);
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

        static IEnumerable<Button> CollectButtons()
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
                    ret.Add(new Button(() => view.DrawNodeEditor(nodeEditorConcreteType))
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
    }
 
    
}