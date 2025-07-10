using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeSetValue: ActionNode, IRequireBlackBoard, IHasPopup
    {
        [PropertyOrder(0)][ShowInInspector][Required]
        Blackboard blackboard;
        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        
        [PropertyOrder(20)]
        public Union ToValue;
        public MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName => "Select Field";
        public string[] PopUpOptions => fieldInfoDic?.Keys.ToArray() ?? new string[]{};
        public int InitialSelectedIndex => selectedIndex;
        public Action<int> SaveSelectedOption => x =>
        {
            selectedIndex = x;
            ToValue.BoardEValueType = 
                Union.Create(fieldInfoDic[selectedOption].FieldType,
                Blackboard.Get(selectedOption)).BoardEValueType;
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
        protected new void OnEnable()
        {
            base.OnEnable();
            OnEnter = () =>
            {
                Blackboard.Set(selectedOption, ToValue.objectVal);
            };
        }
    }
}