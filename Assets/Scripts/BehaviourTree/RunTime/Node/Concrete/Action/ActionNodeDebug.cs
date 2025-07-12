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
    public class ActionNodeDebug : ActionNode, IShowDetail
    {
        public string Content;
        public EDebugType DebugType = EDebugType.Log;

        protected override Action OnEnter => () =>
            {
                switch (DebugType)
                {
                    case EDebugType.Warning:
                        MyDebug.LogWarning(Content, LogType.Tick);
                        break;
                    case EDebugType.Error:
                        MyDebug.LogError(Content, LogType.Tick);
                        break;
                    case EDebugType.Log:
                    default:
                        MyDebug.Log(Content, LogType.Tick);
                        break;
                }
            };

        public string GetDetail()
        {
            return $"Debug {Content}";
        }
    }
}