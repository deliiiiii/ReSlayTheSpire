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
                    TypeCache.EditorToSubNodeDic[abstractNodeEditorType] = new List<Type>();
                    var tBaseType = abstractNodeEditorType.BaseType!.GetGenericArguments()[0];
                    var tSubTypes = tBaseType.SubType();
                    if (!tSubTypes.Any())
                    {
                        TypeCache.EditorToSubNodeDic[abstractNodeEditorType].Add(tBaseType);
                    }
                    else
                    {
                        tSubTypes.ForEach(tSubType =>
                        {
                            //TODO 确保每个RT都有一个对应的Editor吗。好像不用
                            TypeCache.EditorToSubNodeDic[abstractNodeEditorType].Add(tSubType);
                        });
                    }
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
            
            var addButton = new Button(view.Save)
            {
                text = "Save Tree",
                style =
                {
                    width = 200,
                    height = 30,
                    marginLeft = 10
                }
            };
            toolbar.Add(addButton);
            
            var loadButton = new Button(() => view.Load())
            {
                text = "Load Tree",
                style =
                {
                    width = 200,
                    height = 30,
                    marginLeft = 10
                }
            };
            toolbar.Add(loadButton);
        }

        static IEnumerable<Button> CollectButtons()
        {
            List<Button> ret = new();
            var baseType = typeof(NodeBaseEditor<>);
            var nodeTypes = Assembly.GetExecutingAssembly().GetTypes();
            nodeTypes
                // 如 ActionNodeEditor, CompositeNodeEditor, DecoratorNodeEditor
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && !type.IsAbstract
                    // ||
                    // (type is GuardNodeEditor)
                    ) 
                .ForEach(nodeEditorConcrteType =>
                {
                    // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                    ret.Add(new Button(() => view.DrawNodeEditor(nodeEditorConcrteType))
                    {
                        text = nodeEditorConcrteType.Name,
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
 
    
}