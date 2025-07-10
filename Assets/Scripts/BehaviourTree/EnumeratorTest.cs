using System;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace BehaviourTree
{
    public class EnumeratorTest : MonoBehaviour
    {
        int fc => Time.frameCount;
        float t => Time.time;
        string s => $"Frame: {fc}, Time: {t:F4}";
        async void Awake()
        {
            await Task.Delay(2000);
            MyDebug.Log("Awake 1" + s);
            await Task.Delay(2000);
            MyDebug.Log("Awake 2" + s);
            await Task.Yield();
            MyDebug.Log("Awake 3" + s);
            await Task.Delay(2000);
            MyDebug.Log("Awake 4" + s);            
        }

        async Task TestAsync()
        {
            
        }
        //cts = new CancellationTokenSource(); 
        //cts.CancelAfter(1000);
        //cts.Token.Register(() => 
    }
}