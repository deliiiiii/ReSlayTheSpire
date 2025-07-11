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
        static readonly Dictionary<int, BTEditorWindow> windowDic = new();
        static BTGraphView curView;
        static BTEditorWindow curWindow;
        static RootNode curRootNode;
        static BTEditorWindow()
        {
            // 编译前关闭所有已打开窗口
            AssemblyReloadEvents.beforeAssemblyReload += CloseAllWindows;
            // 关掉unity时关闭所有窗口 
            EditorApplication.wantsToQuit += () =>
            {
                CloseAllWindows();
                return true;
            };
        }
        void OnEnable()
        {
            // InitView
            curView = new BTGraphView();
            curView.OnRootNodeDeleted += CloseWindow;
            curView.Load(curRootNode);
            rootVisualElement.Add(curView);
            
            // InitToolbar
            var toolbar = new Toolbar();
            CollectButtons(curView).ForEach(toolbar.Add);
            rootVisualElement.Add(toolbar);
        }
        void OnDisable()
        {
            var kvp = windowDic.FirstOrDefault(kvp => kvp.Value == this);
            windowDic.Remove(kvp.Key);
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is not RootNode node)
                return false;
            curRootNode = node;
            
            // InitWindow
            if (windowDic.TryGetValue(instanceID, out var value))
            {
                curWindow = value;
                curWindow.Focus();
                return true;
            }
            curWindow = CreateWindow<BTEditorWindow>();
            curWindow.titleContent = new GUIContent(curRootNode.name);
            curWindow.minSize = new Vector2(600, 400);
            curWindow.Show();
            windowDic.Add(instanceID, curWindow);
            return true;
        }
        
        static void CloseWindow(int instanceID)
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
        
        static IEnumerable<Button> CollectButtons(BTGraphView fView)
        {
            List<Button> ret = new();
            TypeCache.NodeGeneralTypes.ForEach(nodeGeneralType =>
            {
                // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                ret.Add(new Button(() => fView.DrawNodeEditorWithType(TypeCache.GeneralToSelectionsDic[nodeGeneralType][0]))
                {
                    text = nodeGeneralType.Name,
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