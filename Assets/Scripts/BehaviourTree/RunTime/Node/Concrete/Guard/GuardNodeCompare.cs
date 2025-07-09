using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;

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
    public class GuardNodeCompare : GuardNode, IRequireBlackBoard, IHasPopup
    {
        public Blackboard Blackboard { get; set; }
        [ReadOnly][ShowInInspector]
        public Union FromValue => GetFromValue();
        public Union ToValue;
        public CompareType CompareType;
        
        Dictionary<string, FieldInfo> fieldInfoDic;

        #region Popup
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => fieldInfoDic?.Keys.ToArray();
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x => selectedIndex = x;
        int selectedIndex;
        #endregion
        
        Union GetFromValue()
        {
            if (string.IsNullOrEmpty(Popup.SelectedOption))
                return Union.Null;
            return Union.Create(fieldInfoDic[Popup.SelectedOption].FieldType, Blackboard.Get(Popup.SelectedOption));
        }
        void OnEnable()
        {
            if ((fieldInfoDic ??= new Dictionary<string, FieldInfo>()).Count == 0)
            {
                Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
                {
                    fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
                });
            }
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