using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector]
        RootNode root;
        public static RootNode CreateTree(RootNode root) => Instance.PrivateCreateTree(root);
        public static void Save(string s1, string s2) => Instance.PrivateSave(s1, s2);
        public static RootNode Load(string s1, string s2) => Instance.PrivateLoad(s1, s2);
        
        void PrivateSave(string s1, string s2)
        {
            Saver.Save(s1, s2, root);
        }

        public RootNode PrivateLoad(string s1, string s2)
        {
            return Saver.Load<RootNode>(s1, s2);
        }
        
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