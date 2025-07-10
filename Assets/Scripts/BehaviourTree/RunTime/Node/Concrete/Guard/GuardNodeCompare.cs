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
        [PropertyOrder(0)][ShowInInspector][OnValueChanged(nameof(OnBBChanged))] Blackboard blackboard;
        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        void OnBBChanged()
        {
            fieldInfoDic = new Dictionary<string, FieldInfo>();
            Blackboard?.GetType().GetFields().ForEach(fieldInfo =>
            {
                fieldInfoDic.TryAdd(fieldInfo.Name, fieldInfo);
            });
        }
        
        [PropertyOrder(10)][ShowInInspector][ReadOnly]
        Union fromValue => Blackboard == null || string.IsNullOrEmpty(Popup?.SelectedOption)
            ? Union.Null
            : Union.Create(fieldInfoDic[Popup.SelectedOption].FieldType,
                Blackboard.Get(Popup.SelectedOption));
        [PropertyOrder(20)]
        public Union ToValue;
        [PropertyOrder(30)]
        public CompareType CompareType;
        
        #region Popup
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => fieldInfoDic?.Keys.ToArray() ?? new string[]{};
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x =>
        {
            selectedIndex = x;
            ToValue.BoardEValueType = fromValue.BoardEValueType;
        };
        Dictionary<string, FieldInfo> fieldInfoDic;
        int selectedIndex;
        #endregion
        
        
        void OnEnable()
        {
            OnBBChanged();
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