using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector][SerializeField] RootNode root;
        // public static RootNode StaticRoot
        // {
        //     get => Instance.root;
        //     set => Instance.root = value;
        // }

        void Start()
        {
            Application.targetFrameRate = 10;
            Binder.Update(Tick);
        }

        void Tick(float dt)
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            root.Tick(dt);
        }
    }
}