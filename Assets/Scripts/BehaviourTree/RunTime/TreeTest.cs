using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        public static RootNode Root
        {
            get => Instance.RootProperty;
            set => Instance.RootProperty = value;
        }
        [ShowInInspector] [ReadOnly] RootNode RootProperty { get; set; }
        void Update()
        {
            Tick(Time.deltaTime);
        }

        void Tick(float dt)
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            RootProperty.Tick(dt);
        }
    }
}