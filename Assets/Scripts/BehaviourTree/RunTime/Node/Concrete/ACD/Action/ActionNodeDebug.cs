using System;

namespace BehaviourTree
{
    public enum EDebugType
    {
        Log,
        Warning,
        Error
    }
    
    [Serializable]
    public class ActionNodeDebug : ActionNode
    {
        [DrawnField]
        public string Content;
        [DrawnField]
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