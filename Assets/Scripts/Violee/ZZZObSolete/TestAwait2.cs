// using System;
// using System.Threading.Tasks;
// using UnityEngine;
//
// namespace Violee;
//
// public class TestAwait2 : MonoBehaviour
// {
//     public bool B;
//
//     void Awake()
//     {
//         Application.targetFrameRate = 10;
//     }
//
//     void Update()
//     {
//         MyDebug.Log("Update");
//     }
//
//     async void LateUpdate()
//     {
//         if (B)
//             return;
//         MyDebug.Log("LateUpdate 1");
//         await Func();
//         MyDebug.Log("LateUpdate 2");
//     }
//
//     async Task Func()
//     {
//         await Task.Yield();
//         B = true;
//     }
// }