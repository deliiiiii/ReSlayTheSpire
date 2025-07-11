using System;

namespace BehaviourTree
{
    [Serializable]
    public enum EDebugType
    {
        Log,
        Warning,
        Error
    }
    [Serializable]
    public enum EState
    {
        Failed,
        Succeeded,
        Running,
    }
    [Serializable]
    public enum EChildCountType
    {
        None,
        Single,
        Multiple,
    }
    public enum CompareType
    {
        Equal,
        NotEqual,
        MoreThan,
        LessThan,
        MoreThanOrEqual,
        LessThanOrEqual
    }
}