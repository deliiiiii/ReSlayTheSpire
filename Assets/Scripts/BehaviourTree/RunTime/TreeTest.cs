using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector] RootNode privateRoot;
        public static RootNode Root
        {
            get => Instance.privateRoot;
            set => Instance.privateRoot = value;
        }
        void Update()
        {
            Tick(Time.deltaTime);
        }

        void Tick(float dt)
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            privateRoot.Tick(dt);
        }
    }
}