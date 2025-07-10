using System;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector][SerializeField] RootNode root;
        
        void Start()
        {
            Application.targetFrameRate = 10;
        }

        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                await Tick();
        }

        async Task Tick()
        {
            while (true)
            {
                using var cts = new CancellationTokenSource();
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
                await root.TickAsync(cts);
            }
        }
    }
}