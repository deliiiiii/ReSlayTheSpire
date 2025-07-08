using System;

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
    public class GuardNodeCompare : GuardNode
    {
        public Union FromBlackboard;
        public Union CompareTo;
        
        public CompareType CompareType;
        
        void OnEnable()
        {
            Condition = () =>
            {
                if (FromBlackboard.BoardEValueType == EBoardEValueType.@null || CompareTo.BoardEValueType == EBoardEValueType.@null)
                {
                    // MyDebug.LogError("GuardNodeCompare: FromBlackboard or CompareTo is null");
                    return false;
                }

                return CompareType switch
                {
                    CompareType.Equal => FromBlackboard.Equals(CompareTo),
                    CompareType.NotEqual => !FromBlackboard.Equals(CompareTo),
                    CompareType.MoreThan => FromBlackboard.CompareTo(CompareTo) > 0,
                    CompareType.LessThan => FromBlackboard.CompareTo(CompareTo) < 0,
                    CompareType.MoreThanOrEqual => FromBlackboard.CompareTo(CompareTo) >= 0,
                    CompareType.LessThanOrEqual => FromBlackboard.CompareTo(CompareTo) <= 0,
                    _ => true
                };
            };
        }
    }
}