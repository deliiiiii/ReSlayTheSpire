using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using Object = UnityEngine.Object;

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
        [ReadOnly][ShowInInspector]
        public Union FromValue => GetFromValue();
        public Union ToValue;
        public CompareType CompareType;
        
        Dictionary<string, FieldInfo> fieldInfoDic;
        Observable<int> selectedIndex = new (0);
        string[] options;
        string selectedOption;

        public void DrawPopup()
        {
            if((options?.Length ?? 0) == 0 || selectedIndex == null)
            {
                options = new[] { "No Fields Available" }; 
                return;
            }
            if (selectedIndex >= options.Length)
            {
                selectedIndex.Value = 0;
            }
            selectedIndex.Value = EditorGUILayout.Popup("Select Field", selectedIndex, options);
        }

        Union GetFromValue()
        {
            if (string.IsNullOrEmpty(selectedOption))
                return Union.Null;
            return Union.Create(fieldInfoDic[selectedOption].FieldType, Blackboard.Get(selectedOption));
        }

        void OnOptionChanged(int i)
        {
            selectedOption = options.Length != 0 ? options[i] : string.Empty;
            MyDebug.Log($"OnOptionChanged {i} {selectedOption}");
        }

        void OnEnable()
        {
            if ((fieldInfoDic ??= new Dictionary<string, FieldInfo>()).Count == 0)
            {
                Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
                {
                    fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
                });
                options = fieldInfoDic?.Keys.ToArray();
            }
            selectedIndex.OnValueChangedAfter += OnOptionChanged;
            OnOptionChanged(selectedIndex.Value);
            Condition = () =>
            {
                if (FromValue.BoardEValueType == EBoardEValueType.@null || ToValue.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("SerializedProperty or CompareToValue is null");
                    return false;
                }
                return CompareType switch
                {
                    CompareType.Equal => FromValue.Equals(ToValue),
                    CompareType.NotEqual => !FromValue.Equals(ToValue),
                    CompareType.MoreThan => FromValue.CompareTo(ToValue) > 0,
                    CompareType.LessThan => FromValue.CompareTo(ToValue) < 0,
                    CompareType.MoreThanOrEqual => FromValue.CompareTo(ToValue) >= 0,
                    CompareType.LessThanOrEqual => FromValue.CompareTo(ToValue) <= 0,
                    _ => true
                };
            };
        }
    }
}