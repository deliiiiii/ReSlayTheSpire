#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    [InitializeOnLoad]
    public class BTEditorWindow : EditorWindow
    {
        static readonly Dictionary<string, BTEditorWindow> windowDic = new();
        static BTGraphView curView;
        static BTEditorWindow curWindow;
        static RootNode curRootNode;
        static BTEditorWindow()
        {
            // 编译前关闭所有已打开窗口
            AssemblyReloadEvents.beforeAssemblyReload += CloseAllWindows;
            // 退出PlayMode时关闭所有窗口
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    CloseAllWindows();
                }
            };
            // 关掉unity时关闭所有窗口 
            EditorApplication.wantsToQuit += () =>
            {
                CloseAllWindows();
                return true;
            };
        }
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
        
        void InitView()
        {
            curView = new BTGraphView();
            curView.OnRootNodeDeleted += CloseWindow;
            curView.Load(curRootNode);
            rootVisualElement.Add(curView);
        }

        void InitToolbar()
        {
            var toolbar = new Toolbar();
            CollectButtons(curView).ForEach(toolbar.Add);
            rootVisualElement.Add(toolbar);
        }
        
        static void CloseWindow(string instanceID)
        {
            if (!windowDic.TryGetValue(instanceID, out var value))
                return;
            value.Close();
            windowDic.Remove(instanceID);
        }
        
        static void CloseAllWindows()
        {
            foreach (var kvp in windowDic.ToList())
            {
                kvp.Value.Close();
            }
            windowDic.Clear();
        }
        
        public static void OpenGraph(RootNode rootNode)
        {
            curRootNode = rootNode;
            var rootName = rootNode.Name;
            if (windowDic.TryGetValue(rootName, out var value))
            {
                curWindow = value;
                curWindow.Focus();
                return;
            }
            curWindow = CreateWindow<BTEditorWindow>();
            curWindow.titleContent = new GUIContent(curRootNode.Name);
            curWindow.minSize = new Vector2(600, 400);
            curWindow.Show();
            windowDic.Add(rootName, curWindow);

        }
        
        static IEnumerable<Button> CollectButtons(BTGraphView fView)
        {
            List<Button> ret = new();
            BTTypeCache.NodeGeneralTypes.ForEach(nodeGeneralType =>
            {
                // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                ret.Add(new Button(() => fView.DrawNodeEditorWithType(BTTypeCache.GeneralToSelectionsDic[nodeGeneralType][0]))
                {
                    text = nodeGeneralType.Name.Replace("Node", ""),
                    style =
                    {
                        width = 120,
                        height = 30,
                        marginLeft = ret.Count == 0 ? 10: 0
                    }
                });
            });
            return ret;
        }
    }
}

#endif