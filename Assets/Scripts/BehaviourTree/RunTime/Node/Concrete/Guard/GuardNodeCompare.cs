using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

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
        [ShowInInspector]
        public Blackboard Blackboard { get; set; }
        public Union FromMemberValue; //=> Blackboard.Get(FromMemberName);
        // [ShowInInspector]
        // public object FromMemberValue2 => SelectedOption != string.Empty ? Blackboard?.Get<object>(SelectedOption) : null;
        public Union CompareToValue;
        public CompareType CompareType;
        Dictionary<string, MemberInfo> memberInfoDic;
        
        
        public int selectedIndex = 0;
        string[] Options => memberInfoDic?.Keys.ToArray();
        string SelectedOption => Options.Length == 0 ? string.Empty : Options[selectedIndex] ?? string.Empty;

        public void DrawPopup()
        {
            if ((memberInfoDic ??= new Dictionary<string, MemberInfo>()).Count == 0)
            {
                Blackboard?.GetType().GetMembers().ForEach(memberInfo =>
                {
                    memberInfoDic.TryAdd(memberInfo.Name, memberInfo);
                });
            }
            MyDebug.Log("GuardNodeCompare DrawPopup Options: " + string.Join(", ", Options));
            selectedIndex = EditorGUILayout.Popup("sss", 0, Options);
        }

        void OnEnable()
        {
            Condition = () =>
            {
                if (FromMemberValue.BoardEValueType == EBoardEValueType.@null || CompareToValue.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("GuardNodeCompare: FromBlackboard or CompareTo is null");
                    return false;
                }

                return CompareType switch
                {
                    CompareType.Equal => FromMemberValue.Equals(CompareToValue),
                    CompareType.NotEqual => !FromMemberValue.Equals(CompareToValue),
                    CompareType.MoreThan => FromMemberValue.CompareTo(CompareToValue) > 0,
                    CompareType.LessThan => FromMemberValue.CompareTo(CompareToValue) < 0,
                    CompareType.MoreThanOrEqual => FromMemberValue.CompareTo(CompareToValue) >= 0,
                    CompareType.LessThanOrEqual => FromMemberValue.CompareTo(CompareToValue) <= 0,
                    _ => true
                };
            };
        }
    }
}