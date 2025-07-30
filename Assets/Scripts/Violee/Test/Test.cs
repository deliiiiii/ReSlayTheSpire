using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee.Violee.Test;

public class Test : MonoBehaviour
{
    // Stream<int> s;
    // void Awake()
    // {
    //     s = new Stream<int>(() => i).Where(IsEven);
    // }
    //
    // [SerializeField]
    // int i = 2;
    //
    // public static bool IsEven(int fi) => fi % 2 == 0;
    // public void LogI(int fi) => MyDebug.Log($"i = {i}");
    //
    // async Task LogIAsync(int fi)
    // {
    //     MyDebug.Log($"Start..");
    //     await Task.Delay(1000);
    //     MyDebug.Log($"i = {i}");
    //     await Task.Delay(1000);
    //     MyDebug.Log($"End..");
    // }
    //
    // async Task Test22()
    // {
    //     s.SetTriggerAsync(LogIAsync);
    //     await s.Delay(2000).CallTriggerAsync();
    //     await Task.Delay(1000);
    //     MyDebug.Log($"Test22...");
    //     
    // }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         Test22();
    //     }
    // }
}

public delegate void BuildRenderOperation(
    Action<string> setTip,
    Action<bool> setBuildable,
    Action<Vector2Int> onMove,
    Action<Vector2Int> onConfirm);