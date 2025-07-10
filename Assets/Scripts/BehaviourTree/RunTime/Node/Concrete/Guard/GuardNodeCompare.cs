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
        [ShowInInspector] [PropertyOrder(0)][InlineEditor] Blackboard blackboard;
        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        void OnBlackboardChanged()
        {
            fieldInfoDic = new Dictionary<string, FieldInfo>();
            Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
            {
                fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
            });
            (this as IHasPopup).Init();
        }

        Union fromValue;
        Union FromValue
        {
            get
            {
                if (fromValue != null)
                    return fromValue;
                return fromValue = string.IsNullOrEmpty(Popup.SelectedOption)
                    ? Union.Null
                    : Union.Create(fieldInfoDic[Popup.SelectedOption].FieldType,
                        Blackboard.Get(Popup.SelectedOption));
            }
        }
        
        #region Popup
        [PropertyOrder(10)]
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => fieldInfoDic?.Keys.ToArray() ?? new string[]{};
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x => selectedIndex = x;
        Dictionary<string, FieldInfo> fieldInfoDic;
        int selectedIndex;
        #endregion
        
        [PropertyOrder(20)]
        public Union ToValue;
        [PropertyOrder(30)]
        public CompareType CompareType;
        void OnEnable()
        {
            OnBlackboardChanged();
            Condition = () =>
            {
                if (FromValue.BoardEValueType == EBoardEValueType.@null || ToValue.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("SerializedProperty or CompareToValue is null");
                    return false;
                }
                return CompareType switch
                {
                    CompareType.Equal => FromValue.CompareTo(ToValue) == 0,
                    CompareType.NotEqual => FromValue.CompareTo(ToValue) != 0,
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