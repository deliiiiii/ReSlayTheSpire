using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class EDebugTypeDrawer : OdinValueDrawer<EDebugType>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            
            // var healthField = new IntegerField("Health") { value = entry.SmartValue.Health };
            // healthField.RegisterValueChangedCallback(e => entry.SmartValue.Health = e.newValue);
            //
            // var manaField = new IntegerField("Mana") { value = entry.SmartValue.Mana };
            // manaField.RegisterValueChangedCallback(e => entry.SmartValue.Mana = e.newValue);
            //
            // root.Add(healthField);
            // root.Add(manaField);
            
            
            // EDebugType e = (EDebugType)Property.ValueEntry.WeakSmartValue;
            // // MyDebug.Log($"eee ,{e}");
            //
            // var enumField = new EnumField(Property.Name, e);
            // enumField.RegisterValueChangedCallback(evt =>
            // {
            //     Property.ValueEntry.WeakSmartValue = evt.newValue;
            // });
            
            
            
            Property.ValueEntry.WeakSmartValue = EditorGUILayout.EnumPopup(
                label: Property.Name, 
                selected: (System.Enum)Property.ValueEntry.WeakSmartValue
            );
        }
    }
    
}