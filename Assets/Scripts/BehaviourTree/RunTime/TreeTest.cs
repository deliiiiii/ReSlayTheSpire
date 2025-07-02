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