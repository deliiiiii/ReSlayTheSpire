#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ExTest : MonoBehaviour
    {
        // string[] options = {"Cube", "Sphere", "Plane"};
        // public int index;
        // public Blackboard Board;
        //
        // void Awake()
        // {
        //     index = EditorGUILayout.Popup(index, options);
        // }
        //
        // void Update()
        // {
        //     MyDebug.Log(Board.Get<float>("Float"));
        // }

        // public Blackboard Blackboard;
        // // [ShowInInspector]
        // public SerializedObject fs;
        // [ShowInInspector]
        // public SerializedProperty sp;
        //
        // void Awake()
        // {
        //     fs = new SerializedObject(Blackboard);
        //     sp = fs.FindProperty("FFFFF");
        // }
        //
        // void Update()
        // {
        //     MyDebug.Log($"TTT { sp == null} {sp?.propertyType}");
        // }
    }
}