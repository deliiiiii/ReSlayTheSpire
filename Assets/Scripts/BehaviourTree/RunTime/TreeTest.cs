using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector]
        RootNode root;
        public static RootNode CreateTree(RootNode root) => Instance.PrivateCreateTree(root);
        
#if UNITY_EDITOR
        public static void Save(string s1, string s2) => Instance.PrivateSave(s1, s2);
        public static RootNode Load(string s1, string s2) => Instance.PrivateLoad(s1, s2);
        
        void PrivateSave(string s1, string s2)
        {
            // Saver.Save(s1, s2, root);
            AssetDatabase.CreateAsset(root, $"Assets/{s1}/{s2}.asset");
            root.OnSave(); 
            
            EditorUtility.SetDirty(root); // 标记资源已修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        

        RootNode PrivateLoad(string s1, string s2)
        {
            // return Saver.Load<RootNode>(s1, s2);
            var assetPath = $"Assets/{s1}/{s2}.asset";
            var loadedRoot = AssetDatabase.LoadAssetAtPath<RootNode>(assetPath);
            if (loadedRoot != null)
                return root = loadedRoot;
            Debug.LogError($"Failed to load RootNode from {assetPath}");
            return null;
        }
#endif
        
        void Update()
        {
            Tick(Time.deltaTime);
        }

        RootNode PrivateCreateTree(RootNode rootNode)
        {
            return root = rootNode;
        }
        void Tick(float dt)
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            root.Tick(dt);
        }
    }
}