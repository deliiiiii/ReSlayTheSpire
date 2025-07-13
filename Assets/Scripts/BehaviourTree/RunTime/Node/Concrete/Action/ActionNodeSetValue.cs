using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeSetValue: ActionNode, IShowDetail
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            OnDelayEnd += () => Blackboard?.Set(SelectedOption, ToValue.GetValue());
        }
        
        [PropertyOrder(0)][Required] [CanBeNull] 
        public Blackboard Blackboard;
        [PropertyOrder(20)]
        public Union ToValue;
        Dictionary<string, FieldInfo> fieldInfoDic => Blackboard?.GetFieldInfoDic();
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
        public new string GetDetail()
        {
            return $"{base.GetDetail()}{Blackboard?.name ?? "null"}.{SelectedOption} = {ToValue.GetValue()}";
        }
    }
}