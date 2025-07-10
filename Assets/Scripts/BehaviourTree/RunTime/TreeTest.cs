using System;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviourTree
{
    public class TreeTest : Singleton<TreeTest>
    {
        [ShowInInspector][SerializeField] RootNode root;

        CancellationTokenSource cts;
        public int TarFrameRate = 2;
        void Start()
        {
            Application.targetFrameRate = TarFrameRate;
            cts = new CancellationTokenSource();
            Tick();
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    MyDebug.LogWarning("Exiting Play Mode, cancelling cts", LogType.Tick); 
                    cts?.Cancel();
                }
            };
#endif
        }

        void OnApplicationQuit()
        {
            cts?.Cancel();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                cts?.Cancel();
        }

        async void Tick()
        {
            while (!cts.IsCancellationRequested)
            {
                MyDebug.Log($"----------Start Tick----------", LogType.Tick);
                await root.TickAsync(cts);
                await Task.Yield();
            }
        }
    }
}