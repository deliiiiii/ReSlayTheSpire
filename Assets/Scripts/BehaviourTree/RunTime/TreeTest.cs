using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector][SerializeField] RootNode root;
        public int TarFrameRate = 10;
        public bool ShowStartTick = true;
        void Start()
        {
            Application.targetFrameRate = TarFrameRate;
            Binder.Update(Tick);
        }

        void Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            root.Tick(dt);
        }
    }
}