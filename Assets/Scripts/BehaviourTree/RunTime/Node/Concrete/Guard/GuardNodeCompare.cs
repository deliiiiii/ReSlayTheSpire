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
        [PropertyOrder(0)][ShowInInspector][InlineEditor] Blackboard blackboard;
        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        [ShowInInspector]
        Union fromValue;
        
        #region Popup
        [PropertyOrder(10)]
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => FieldInfoDic?.Keys.ToArray() ?? new string[]{};
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x =>
        {
            selectedIndex = x;
            fromValue = string.IsNullOrEmpty(Popup.SelectedOption)
                ? Union.Null
                : Union.Create(FieldInfoDic[Popup.SelectedOption].FieldType,
                    Blackboard.Get(Popup.SelectedOption));
            ToValue.BoardEValueType = fromValue.BoardEValueType;
        };

        Dictionary<string, FieldInfo> fieldInfoDic;
        public Dictionary<string, FieldInfo> FieldInfoDic
        {
            get
            {
                if ((fieldInfoDic?.Count ?? 0) == 0)
                {
                    fieldInfoDic = new Dictionary<string, FieldInfo>();
                    Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
                    {
                        fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
                    });
                }
                return fieldInfoDic;
            }
        }
        int selectedIndex;
        #endregion
        
        [PropertyOrder(20)]
        public Union ToValue;
        [PropertyOrder(30)]
        public CompareType CompareType;
        void OnEnable()
        {
            Condition = () =>
            {
                if (fromValue.BoardEValueType == EBoardEValueType.@null || ToValue.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("SerializedProperty or CompareToValue is null");
                    return false;
                }
                return CompareType switch
                {
                    CompareType.Equal => fromValue.CompareTo(ToValue) == 0,
                    CompareType.NotEqual => fromValue.CompareTo(ToValue) != 0,
                    CompareType.MoreThan => fromValue.CompareTo(ToValue) > 0,
                    CompareType.LessThan => fromValue.CompareTo(ToValue) < 0,
                    CompareType.MoreThanOrEqual => fromValue.CompareTo(ToValue) >= 0,
                    CompareType.LessThanOrEqual => fromValue.CompareTo(ToValue) <= 0,
                    _ => true
                };
            };
        }
    }
}