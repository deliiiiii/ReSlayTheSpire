using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{
    public enum CompareType
    {
        Equal,
        NotEqual,
        MoreThan,
        LessThan,
        MoreThanOrEqual,
        LessThanOrEqual
    }
    [Serializable]
    public class GuardNodeCompare : GuardNode, IRequireBlackBoard
    {
        public Blackboard Blackboard { get; set; }
        // [ShowInInspector]
        SerializedObject fs;
        [SerializeField] public SerializedProperty sp;
        [HideInInspector]
        public SerializedProperty CompareToValue;
        public CompareType CompareTypeeeeeeeeeeeeeeeee;
        
        Dictionary<string, FieldInfo> fieldInfoDic;
        [OnValueChanged(nameof(OnOptionChanged))]
        int selectedIndex = 0;
        string[] options;
        

        public void DrawPopup()
        {
            if ((fieldInfoDic ??= new Dictionary<string, FieldInfo>()).Count == 0)
            {
                Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
                {
                    fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
                });
                options = fieldInfoDic?.Keys.ToArray();
            }
            if (Blackboard != null && fs == null)
            {
                fs = new SerializedObject(Blackboard);
            }
            selectedIndex = EditorGUILayout.Popup("Select Field", selectedIndex, options);
            OnOptionChanged();
        }

        void OnOptionChanged()
        {
            var selectedOption = options.Length != 0 ? options[selectedIndex] : string.Empty;
            sp = fs?.FindProperty(selectedOption);
        }

        void OnEnable()
        {
            fieldInfoDic = null;
            Condition = () =>
            {

                return CompareTypeeeeeeeeeeeeeeeee switch
                {
                    CompareType.Equal => sp.Equals(CompareToValue),
                    CompareType.NotEqual => !sp.Equals(CompareToValue),
                    // CompareType.MoreThan => fromSerializedPro.CompareTo(CompareToValue) > 0,
                    // CompareType.LessThan => fromSerializedPro.CompareTo(CompareToValue) < 0,
                    // CompareType.MoreThanOrEqual => fromSerializedPro.CompareTo(CompareToValue) >= 0,
                    // CompareType.LessThanOrEqual => fromSerializedPro.CompareTo(CompareToValue) <= 0,
                    _ => true
                };
            };
        }
    }
}