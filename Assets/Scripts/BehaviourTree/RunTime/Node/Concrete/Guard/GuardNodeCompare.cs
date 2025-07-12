using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

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
    public class GuardNodeCompare : GuardNode, IShowDetail
    {
        protected override Func<bool> Condition => () =>
            {
                if (fromValue.BoardEValueType == EBoardEValueType.@null || ToValue.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("fromValue or ToValue is null");
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

        [PropertyOrder(0)][Required] [CanBeNull]
        public Blackboard Blackboard;
        
        [PropertyOrder(10)][ShowInInspector][ReadOnly]
        Union fromValue => Blackboard == null || string.IsNullOrEmpty(SelectedOption)
            ? Union.Null
            : Union.Create(fieldInfoDic[SelectedOption].FieldType,
                Blackboard.Get(SelectedOption));
        [PropertyOrder(20)]
        public Union ToValue;
        [PropertyOrder(30)]
        public CompareType CompareType;
        
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
        [ValueDropdown(nameof(GetOptions))][OnValueChanged(nameof(OnOptionChanged))]
        public string SelectedOption;
        List<string> GetOptions()
        {
            return fieldInfoDic?.Keys.ToList() ?? new List<string>();
        }

        void OnOptionChanged()
        {
            ToValue.BoardEValueType =
                Union.ConvertType(fieldInfoDic[SelectedOption].FieldType);
        }
        public string GetDetail()
        {
            string compareSymbol = CompareType switch
            {
                CompareType.Equal => "==",
                CompareType.NotEqual => "!=",
                CompareType.MoreThan => ">",
                CompareType.LessThan => "<",
                CompareType.MoreThanOrEqual => ">=",
                CompareType.LessThanOrEqual => "<=",
                _ => "?"
            };
            return $"{Blackboard?.name ?? "null"}.{SelectedOption}:{fromValue.GetValue()} {compareSymbol} {ToValue.GetValue()}";
        }
    }
}