using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector][SerializeField] RootNode root;
        
        async void Start()
        {
            Application.targetFrameRate = 10;
            while (true)
            {
                await Tick();
            }
        }

        async Task Tick()
        {
            MyDebug.Log("----------Start Tick----------", LogType.Tick);
            await root.TickAsync();
        }
    }
}