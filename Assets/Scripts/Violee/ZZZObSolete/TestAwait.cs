// using System;
// using System.Threading.Tasks;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Violee.Violee.Test;
//
// public class TestAwait : MonoBehaviour
// {
//     // Stream<int> s;
//     // void Awake()
//     // {
//     //     s = new Stream<int>(() => i).Where(IsEven);
//     // }
//     //
//     // [SerializeField]
//     // int i = 2;
//     //
//     // public static bool IsEven(int fi) => fi % 2 == 0;
//     // public void LogI(int fi) => MyDebug.Log($"i = {i}");
//     //
//     // async Task LogIAsync(int fi)
//     // {
//     //     MyDebug.Log($"Start..");
//     //     await Task.Delay(1000);
//     //     MyDebug.Log($"i = {i}");
//     //     await Task.Delay(1000);
//     //     MyDebug.Log($"End..");
//     // }
//     //
//     // async Task Test22()
//     // {
//     //     s.SetTriggerAsync(LogIAsync);
//     //     await s.Delay(2000).CallTriggerAsync();
//     //     await Task.Delay(1000);
//     //     MyDebug.Log($"Test22...");
//     //     
//     // }
//     // void Update()
//     // {
//     //     if (Input.GetKeyDown(KeyCode.T))
//     //     {
//     //         Test22();
//     //     }
//     // }
//
//     // async Task Test()
//     // {
//     //     var ret1 = await Task.Run(async () =>
//     //     {
//     //         await Task.Yield();
//     //         return 42;
//     //     });
//     //     
//     //     var ret2 = await Func2Async();
//     // }
//     //
//     // static async Task<int> Func2Async()
//     // {
//     //     await Task.Yield();
//     //     return 42;
//     // }
//
//
//     // [Button]
//     // async Task Test()
//     // {
//     //     try
//     //     {
//     //         await Task.Run(async () =>
//     //         {
//     //             await Task.Delay(2000);
//     //             var go = new GameObject("TestAwait");
//     //             Debug.Log("Created GameObject");
//     //         });
//     //     }
//     //     catch (Exception e)
//     //     {
//     //         MyDebug.LogError($"Exception: {e}");
//     //         throw;
//     //     }
//     //     
//     // }
//     //
//     // static Task<int> Func2Async()
//     // {
//     //     return Task.FromResult(42);
//     // }
//
//     
//     [Button]
//     static void TestAsync0()
//     {
//         Instantiate(null);
//         TestAsync1();
//         TestAsync2();
//     }
//
//     [Button]
//     static async void TestAsync1()
//     {
//         try
//         {
//             MyDebug.Log(nameof(TestAsync1));
//             throw new Exception("TestAsync1 Error!");
//         }
//         catch (Exception e)
//         {
//             MyDebug.LogError($"Exception: {e}");
//             throw;
//         }
//     }
//
//     // void StartCo()
//     // {
//     //     StartCoroutine(nameof(TestCo));
//     // }
//     // IEnumerator TestCo()
//     // {
//     //     yield break;
//     // }
//     
//     [Button]
//     static async Task TestAsync2()
//     {
//        
//         try
//         {
//             // await Task.CompletedTask;
//             MyDebug.Log(nameof(TestAsync1));
//             throw new Exception("TestAsync1 Error!");
//         }
//         catch (Exception e)
//         {
//             MyDebug.LogError($"Exception: {e}");
//             throw;
//         }
//     }
// }
//
// public delegate void BuildRenderOperation(
//     Action<string> setTip,
//     Action<bool> setBuildable,
//     Action<Vector2Int> onMove,
//     Action<Vector2Int> onConfirm);