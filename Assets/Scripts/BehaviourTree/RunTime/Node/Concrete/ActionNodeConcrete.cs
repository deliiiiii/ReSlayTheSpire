using System;
using Sirenix.OdinInspector;

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
        public ActionNodeDebug() { }
        public ActionNodeDebug(string content, EDebugType debugType = EDebugType.Log)
        {
            Content = content;
            DebugType = debugType;
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
    [Serializable]
    public class ActionNodeDelay : ActionNode
    {
        [DrawnField]
        public float DelaySeconds;
        [ShowInInspector][ReadOnly]
        float timer;
        public ActionNodeDelay(){}
        public ActionNodeDelay(float delaySeconds)
        {
            DelaySeconds = delaySeconds;
            OnEnter = () => timer = 0;
            OnContinue = dt =>
            {
                timer += dt;
                isFinished = timer >= DelaySeconds;
            };
        }

        public override string ToString()
        {
            return $"Delay {DelaySeconds}s";
        }
    }
    
    public class ActionNodeSet<T> : ActionNode
    {
        [DrawnField]
        public T TarValue;
        public ActionNodeSet(){}
        public ActionNodeSet(T tarValue, Action<T> setter)
        {
            this.TarValue = tarValue;
            OnContinue = _ =>
            {
                setter(this.TarValue);
                isFinished = true;
            };
        }

        public override string ToString()
        {
            return $"Set to {TarValue}";
        }
    }
}