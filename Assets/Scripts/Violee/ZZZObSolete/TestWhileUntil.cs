// using System;
// using System.Threading.Tasks;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Violee;
//
// public class TestWhileUntil : MonoBehaviour
// {
//     int i = 0;
//
//     [Button]
//     public void AddInt()
//     {
//         i++;
//     }
//
//     Action<int> DebugAct = x => MyDebug.Log(x);
//     void Awake()
//     {
//         _ = this.Bind(() => i).WhileUntil(x => x >= 5, DebugAct);
//     }
// }
//
// public static class WhileExt
// {
//     public static async Task<T> WhileUntil<T>(this Func<T> self, Func<T, bool> guard, Action<T> update)
//     {
//         while (true)
//         {
//             var s = self();
//             if (guard(s)) return s;
//             update(s);
//             await Task.Yield();
//         }
//     }
// }