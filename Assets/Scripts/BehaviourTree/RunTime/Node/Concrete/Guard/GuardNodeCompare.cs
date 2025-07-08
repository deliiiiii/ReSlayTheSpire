using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    public class GuardNodeCompare : GuardNode, IRequireBlackBoard
    {
        public Blackboard Blackboard { get; set; }
        public string FromMemberName;
        public Union FromMemberValue; //=> Blackboard.Get(FromMemberName);
        [ShowInInspector]
        public object FromMemberValue2 => FromMemberName != null ? Blackboard?.Get<object>(FromMemberName) : null;
        public Union CompareToValue;
        public CompareType CompareType;
        Dictionary<string, MemberInfo> memberInfoDic = new();
        List<string> GetMemberList() => memberInfoDic.Keys.ToList();
        
        void OnEnable()
        {
            memberInfoDic ??= new Dictionary<string, MemberInfo>();
            Blackboard?.GetType().GetMembers().ForEach(memberInfo =>
            {
                memberInfoDic.TryAdd(memberInfo.Name, memberInfo);
            });
            
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