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
                    && type.IsAbstract
                    && type.IsGenericType
                ) 
                .ForEach(generic =>
                {
                    DropDownFieldDataCache.DDDic[generic] = new List<Type>();
                    var tSubTypes = generic.GetGenericArguments()[0].BaseType.SubType();
                    tSubTypes.ForEach(tSubType =>
                    {
                        // var genericWithSubT = generic.MakeGenericType(tSubType);
                        // var subTSpecific = generic.MakeGenericType(tSubType).FirstSubType();
                        // if (subTSpecific == null)
                        // {
                        //     MyDebug.LogError($"2 {generic} {tSubType} than subTSpecific is null!!");
                        //     return;
                        // }
                        
                        //TODO 确保每个RT都有一个对应的Editor吗。好像不用
                        DropDownFieldDataCache.DDDic[generic].Add(tSubType);
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
                // 继承于NodeBaseEditor<>的非抽象类
                .Where(type =>  
                    type.InheritsFrom(baseType)
                    && type != baseType
                    && type.IsAbstract
                    && type.IsGenericType
                    ) 
                .ForEach(tGeneric =>
                {
                    var typeTSubType = tGeneric.GetGenericArguments()[0].BaseType.FirstSubType();
                    var subTSpecific = tGeneric.MakeGenericType(typeTSubType).FirstSubType();
                    // MyDebug.Log($"1 {tGeneric} {typeTSubType} {subTSpecific}");
                    ret.Add(new Button(() => view.DrawNodeEditor(subTSpecific))
                    {
                        // text = nameof(BTGraphView.CreateNode) + "<" + subTSpecific.Name + ">",
                        text = tGeneric.Name[..^2],
                        style =
                        {
                            width = 200,
                            height = 30,
                            marginLeft = ret.Count == 0 ? 10: 0,
                            // marginRight = 5 + (count++) * 20,
                            // marginTop = 2 + (count++) * 20,
                            // marginBottom = 2 + (count++) * 20
                        }
                    });
                    // MyDebug.Log($"Adding button for {tSpecific.Name}<{tSpecific.GetGenericArguments()[0].Name}> sub{subTSpecific.Name}");
                    
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
        public static Type FirstSubType(this Type parentType)
        {
            return parentType.SubType().FirstOrDefault();
        }
        
        public static IEnumerable<Type> SubType(this Type parentType)
        {
            return parentType.Assembly.GetTypes().Where(x => x.IsSubclassOf(parentType));
        }
        
    }
}