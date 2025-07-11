using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeSetValue: ActionNode, IShowDetail
    {
        public ActionNodeSetValue()
        {
            OnEnter = () =>
            {
                Blackboard?.Set(SelectedOption, ToValue.GetValue());
            };
        }
        
        [PropertyOrder(0)][Required]
        public Blackboard Blackboard;

        [PropertyOrder(20)]
        public Union ToValue;
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
            return $"{Blackboard?.name ?? "null"}.{SelectedOption} = {ToValue.GetValue()}";
        }
    }
}