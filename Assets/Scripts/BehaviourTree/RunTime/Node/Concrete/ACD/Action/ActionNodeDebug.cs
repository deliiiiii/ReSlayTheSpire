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
    public class ActionNodeDebug : ActionNode
    {
        public string Content;
        public EDebugType DebugType = EDebugType.Log;

        void OnEnable()
        {
            OnEnter = () =>
            {
                switch (DebugType)
                {
                    case EDebugType.Log:
                        MyDebug.Log(Content, LogType.Tick);
                        break;
                    case EDebugType.Warning:
                        MyDebug.LogWarning(Content, LogType.Tick);
                        break;
                    case EDebugType.Error:
                        MyDebug.LogError(Content, LogType.Tick);
                        break;
                }
            };
            OnContinue = _ => isFinished = true;
        }
        
        public override string ToString()
        {
            return $"{Content}";
        }
    }
}