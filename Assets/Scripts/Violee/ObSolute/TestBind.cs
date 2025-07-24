// using System;
// using UnityEngine;
//
// namespace Test;
//
// public class TestBind : MonoBehaviour
// {
//     public int Count = 1000;
//     int c;
//     
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.B))
//         {
//             var ob = new Observable<int>(111);
//             for (int i = 0; i < Count; i++)
//             {
//                 Binder.From(ob).To(v => c = v);
//             }
//         }
//     }
// }