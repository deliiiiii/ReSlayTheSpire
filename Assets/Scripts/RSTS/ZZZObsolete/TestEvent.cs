// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace RSTS.Test;
//
// public class TestEvent : MonoBehaviour
// {
//     public event Action<int>? OnIntAChanged;
//     [field: SerializeField]
//     public int IntA
//     {
//         get => field;
//         set
//         {
//             if (value == field)
//             {
//                 return;
//             }
//
//             field = value;
//             OnIntAChanged?.Invoke(field);
//         }
//     }
//     
//     [Button]
//     public void Test()
//     {
//         IntA++;
//     }
// }
//
//
// [Serializable]
// public class TestData
// {
//     
// }