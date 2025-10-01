using System;
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
        static GraphData curGraphData;
        // static BTEditorWindow()
        // {
            // 编译前关闭所有已打开窗口
            // AssemblyReloadEvents.beforeAssemblyReload += CloseAllWindows;
            // 关掉unity时关闭所有窗口 
            // EditorApplication.wantsToQuit += () =>
            // {
                // CloseAllWindows();
                // return true;
            // };
        // }
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is not GraphData graphData)
                return false;
            curGraphData = graphData;
            if (windowDic.TryGetValue(instanceID, out var value))
            {
                value.Focus();
                return true;
            }

            var addedWindow = CreateWindow<BTEditorWindow>();
            addedWindow.Show();
            return true;
        }
        static IEnumerable<Button> CollectButtons(BTGraphView fView)
        {
            List<Button> ret = new();
            TypeCache.NodeGeneralTypes.ForEach(nodeGeneralType =>
            {
                // MyDebug.Log($"Adding button for {abstractNodeEditorType.Name}");
                ret.Add(new Button(() => fView.DrawNewNodeEditor(TypeCache.GeneralToSelectionsDic[nodeGeneralType][0]))
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
        
        GraphData graphData;
        void OnEnable()
        {
            graphData ??= curGraphData;
            titleContent = new GUIContent(graphData.name);
            minSize = new Vector2(600, 400);
            
            var graphView = new BTGraphView();
            graphView.Load(graphData);
            rootVisualElement.Add(graphView);
            
            var toolbar = new Toolbar();
            CollectButtons(graphView).ForEach(toolbar.Add);
            rootVisualElement.Add(toolbar);
            
            windowDic.TryAdd(graphData.GetInstanceID(), this);
        }
        void OnDisable()
        {
            windowDic.Remove(graphData.GetInstanceID());
        }
        
        // static void CloseAllWindows()
        // {
        //     foreach (var kvp in windowDic.ToList())
        //     {
        //         kvp.Value.Close();
        //     }
        //     windowDic.Clear();
        // }
        // static void CloseWindow(int instanceID)
        // {
        //     if (!windowDic.TryGetValue(instanceID, out var value))
        //         return;
        //     value.Close();
        //     windowDic.Remove(instanceID);
        // }
    }
}