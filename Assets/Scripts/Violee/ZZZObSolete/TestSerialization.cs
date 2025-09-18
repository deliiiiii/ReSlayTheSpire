// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using Sirenix.Serialization;
// using UnityEngine;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif
//
// namespace Violee;
//
// public class TestSerialization : SerializedMonoBehaviour
// {
//     [OdinSerialize][NonSerialized]
//     public required TestS TestS;
//
//     [OdinSerialize][NonSerialized]
//     public required MyGenericClass<int> M;
//     
//     [OdinSerialize][NonSerialized]
//     public required MyGenericClass<List<List<int>>> M2;
//     
//     [OdinSerialize][NonSerialized]
//     public required MyGenericClass<List<int>> M3;
//     
//     [OdinSerialize][NonSerialized]
//     public int IntValue;
//     
//     [OdinSerialize][NonSerialized]
//     public required List<List<int>> L;
//
//     void OnGUI()
//     {
//         if (GUILayout.Button("TestGUI"))
//         {
//             MyDebug.Log("TestGUIBtn Clicked");
//         }
//     }
// }
//
//
// [Serializable] // 试图让它可序列化
// public class MyGenericClass<T>
// {
//     public required T Value;
// }
//
//
// #if UNITY_EDITOR
//
// [CustomEditor(typeof(TestSerialization))]
//
// public class TestSerializationEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//         GUILayout.BeginHorizontal();
//             if (GUILayout.Button("DebugTest"))
//             {
//                 MyDebug.Log("Test 1");
//             }
//
//             if (GUILayout.Button("DebugTest2"))
//             {
//                 MyDebug.Log("Test 2");
//             }
//         EditorGUILayout.EndHorizontal();
//         
//         
//         if (GUILayout.Button("DebugTest3"))
//         {
//             MyDebug.Log("Test 3");
//         }
//
//         if (Event.current.Equals(Event.KeyboardEvent("Q")) && Event.current.type == EventType.KeyDown)
//         {
//             MyDebug.Log("Q Pressed");
//             
//         }
//     }
// }
// #endif