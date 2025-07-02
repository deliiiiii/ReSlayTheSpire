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

namespace BehaviourTree
{
    public class BTEditorWindow : EditorWindow
    {
        static BTGraphView view;
        [MenuItem("BTGraph/Open Graph Editor")]
        public static void OnOpen()
        {
            InitWindow();
        }
        void OnEnable()
        {
            InitDropDownDicCache();
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

        static void InitDropDownDicCache()
        {
            var baseType = typeof(NodeBaseEditor<>);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            nodeTypes
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && !type.IsAbstract
                )
                // abstractNodeEditorType: ActionNodeEditor or CompositeNodeEditor or DecoratorNodeEditor ...
                .ForEach(abstractNodeEditorType =>
                {
                    DropDownFieldDataCache.DDDic[abstractNodeEditorType] = new List<Type>();
                    var tSubTypes = abstractNodeEditorType.BaseType!.GetGenericArguments()[0].SubType();
                    tSubTypes.ForEach(tSubType =>
                    {
                        //TODO 确保每个RT都有一个对应的Editor吗。好像不用
                        DropDownFieldDataCache.DDDic[abstractNodeEditorType].Add(tSubType);
                    });
                });
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
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && !type.IsAbstract
                    // ||
                    // (type is GuardNodeEditor)
                    ) 
                .ForEach(abstractNodeEditorType =>
                {
                    // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                    ret.Add(new Button(() => view.DrawNodeEditor(abstractNodeEditorType))
                    {
                        text = abstractNodeEditorType.Name,
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
        
        void DestructGraph()
        {
            rootVisualElement.Remove(view);
        }
        
    }
 
    public static class ReflectionExtensions
    {
        // public static Type FirstSubType(this Type parentType)
        // {
        //     return parentType.SubType().FirstOrDefault();
        // }
        
        public static IEnumerable<Type> SubType(this Type parentType)
        {
            return parentType.Assembly.GetTypes().Where(x => x.IsSubclassOf(parentType));
        }
        
    }
}