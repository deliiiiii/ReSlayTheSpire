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
        [PropertyOrder(0)][ShowInInspector][Required]
        Blackboard blackboard;
        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        
        [PropertyOrder(10)][ShowInInspector][ReadOnly]
        Union fromValue => Blackboard == null || string.IsNullOrEmpty(selectedOption)
            ? Union.Null
            : Union.Create(fieldInfoDic[selectedOption].FieldType,
                Blackboard.Get(selectedOption));
        [PropertyOrder(20)]
        public Union ToValue;
        [PropertyOrder(30)]
        public CompareType CompareType;
        
        #region Popup
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => fieldInfoDic.Keys.ToArray();
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x =>
        {
            selectedIndex = x;
            ToValue.BoardEValueType = fromValue.BoardEValueType;
        };

        Dictionary<string, FieldInfo> fieldInfoDic => GetFieldInfoDic();
        Dictionary<string, FieldInfo> GetFieldInfoDic()
        {
            var ret = new Dictionary<string, FieldInfo>();
            if (Blackboard == null)
                return ret;
            Blackboard.GetType().GetFields().ForEach(fieldInfo =>
            {
                ret.TryAdd(fieldInfo.Name, fieldInfo);
            });
            return ret;
        }
         
        int selectedIndex;
        string selectedOption => PopUpOptions[selectedIndex];
        #endregion
        
        
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