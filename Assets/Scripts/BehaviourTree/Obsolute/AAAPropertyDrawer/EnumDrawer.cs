// using System;
// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace BehaviourTree
// {
//     public class EnumDrawer : OdinValueDrawer<Enum>
//     {
//         protected override void DrawPropertyLayout(GUIContent label)
//         {
//             Property.ValueEntry.WeakSmartValue = EditorGUILayout.EnumPopup(
//                 label: Property.Name, 
//                 selected: (Enum)Property.ValueEntry.WeakSmartValue
//             );
//         }
//     }
//     
// }w