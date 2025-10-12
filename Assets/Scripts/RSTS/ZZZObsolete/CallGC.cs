// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace RSTS.Test;
//
// public class CallGC : MonoBehaviour
// {
//     
//     [Button]
//     public void CallGCFunc()
//     {
//         // GC.Collect();
//         for (int i = 0; i < 100; i++)
//         {
//             var bc = new BigClass();
//         }
//     }
// }
//
// class BigClass
// {
//     int[] bigArr = new int[1024 * 1024 * 10];
// }