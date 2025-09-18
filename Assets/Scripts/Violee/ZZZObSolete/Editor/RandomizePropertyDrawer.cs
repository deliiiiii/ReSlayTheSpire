using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Violee.Editor;


[CustomPropertyDrawer(typeof(RandomizeFloatAttribute))]
public class RandomizePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // return base.GetPropertyHeight(property, label) + deltaY;
        return deltaY;
    }

    float deltaY = 30f; 

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
            // 使用EditorGUI及position
            EditorGUI.LabelField(position, label, new GUIContent(property.floatValue.ToString()));
            // 使用EditorGUILayout
            // EditorGUILayout.LabelField(label, new GUIContent(property.floatValue.ToString()));
            Rect buttonRect = new Rect(position.x + position.width * 0.7f, position.y, position.width * 0.3f, deltaY);
            if (GUI.Button(buttonRect, "Randomize"))
            { 
                RandomizeFloatAttribute attr = (attribute as RandomizeFloatAttribute)!;
                property.floatValue = Random.Range(attr.Min, attr.Max);
                // property.serializedObject.ApplyModifiedProperties();
            }

            List<int> l = [1, 2, 3];
            l.RemoveAt(0);
        EditorGUI.EndProperty();
    }
}