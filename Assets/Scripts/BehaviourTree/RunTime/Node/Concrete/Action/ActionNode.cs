using System;
using System.Text;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

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
    public enum EDelayType
    {
        Frames,
        Seconds
    }
    
    [Serializable]
    public class ActionNode : NodeBase, IShowDetail
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        // [CanBeNull] protected Action OnEnter;
        [CanBeNull] protected Func<float, EState> OnContinue;
        [CanBeNull] protected Action OnDelayEnd;

        #region Debug
        [OnValueChanged(nameof(OnDebugSettingsChanged))]
        public bool HasDebug;
        [ShowIf(nameof(HasDebug))] public string DebugContent;
        [ShowIf(nameof(HasDebug))] public EDebugType DebugType = EDebugType.Log;
        Action debugOnDelayEnd;
        #endregion


        #region Delay
        [OnValueChanged(nameof(OnDelaySettingsChanged))]
        public bool HasDelay;
        [ShowIf(nameof(HasDelay))][OnValueChanged(nameof(OnDelaySettingsChanged))]
        public EDelayType DelayType;
        
        bool isDelayFrames => HasDelay && DelayType == EDelayType.Frames;
        [ShowIf(nameof(isDelayFrames))]
        public int DelayFrames;
        [ReadOnly][ShowIf(nameof(isDelayFrames))]
        public int DelayFramesTimer;
        
        bool isDelaySeconds => HasDelay && DelayType == EDelayType.Seconds;
        [ShowIf(nameof(isDelaySeconds))]
        public float DelaySeconds;
        [ReadOnly][ShowIf(nameof(isDelaySeconds))]
        public float DelaySecondsTimer;
        
        Func<float, EState> framesOnContinue;
        Func<float, EState> secondsOnContinue;
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            debugOnDelayEnd = () =>
            {
                switch (DebugType)
                {
                    case EDebugType.Warning:
                        MyDebug.LogWarning(DebugContent, LogType.Tick);
                        break;
                    case EDebugType.Error:
                        MyDebug.LogError(DebugContent, LogType.Tick);
                        break;
                    case EDebugType.Log:
                    default:
                        MyDebug.Log(DebugContent, LogType.Tick);
                        break;
                }
            };
            framesOnContinue = dt =>
            {
                DelayFramesTimer++;
                if (DelayFramesTimer >= DelayFrames)
                    return EState.Succeeded;
                return EState.Running;
            };
            secondsOnContinue = dt =>
            {
                DelaySecondsTimer += dt;
                if(DelaySecondsTimer >= DelaySeconds)
                    return EState.Succeeded;
                return EState.Running;
            };
            OnDebugSettingsChanged();
            OnDelaySettingsChanged();
        }

        void OnDebugSettingsChanged()
        {
            if (!HasDebug)
            {
                OnDelayEnd -= debugOnDelayEnd;
                return;
            }
            OnDelayEnd += debugOnDelayEnd;
        }
        void OnDelaySettingsChanged()
        {
            if (!HasDelay)
            {
                OnContinue = _ => EState.Succeeded;
                return;
            }

            OnContinue = null;
            switch (DelayType)
            {
                case EDelayType.Frames:
                    OnContinue += framesOnContinue;
                    break;
                case EDelayType.Seconds:
                    OnContinue += secondsOnContinue;
                    break;
                default:
                    break;
            }
        }

        protected override void OnReset()
        {
            base.OnReset();
            DelayFramesTimer = 0;
            DelaySecondsTimer = 0;
        }
        protected override EState Tick(float dt)
        {
            // 上一帧完成，这一帧才重置
            if (State.Value == EState.Succeeded)
            {
                RecursiveDo(CallReset);
            }
            // if (State.Value == EState.Failed)
            // {
            //     OnEnter?.Invoke();
            // }
            var ret = OnContinue?.Invoke(dt) ?? EState.Succeeded;
            if (ret == EState.Succeeded)
            {
                OnDelayEnd?.Invoke();
            }
            return ret;
        }
        
        #region IShowDetail
        public virtual string GetDetail()
        {
            StringBuilder sb = new StringBuilder();
            if (HasDebug)
                sb.AppendLine(GetDebugDetail());
            if (HasDelay)
            {
                if(isDelayFrames)
                    sb.AppendLine(GetDelayFramesDetail());
                else if (isDelaySeconds)
                    sb.AppendLine(GetDelaySecondsDetail());
            }
            return sb.ToString();
        }
        
        string GetDebugDetail()
        {
            return $"Debug {DebugContent}";
        }

        string GetDelayFramesDetail()
        {
            return $"Delay {DelayFramesTimer}/{DelayFrames}f";
        }
        string GetDelaySecondsDetail()
        {
            string t = DelaySecondsTimer switch
            {
                < 1 => $"{DelaySecondsTimer * 1000:F0}",
                < 60 => $"{DelaySecondsTimer:F2}",
                _ => $"{DelaySecondsTimer / 60:F2}",
            };
            string tTanni = (Timer: DelaySecondsTimer, DelaySeconds) switch
            {
                (<= 1,<= 1) or
                    (>1 and <=60,>1 and <=60) or
                    (>60 and <=3600,>60 and <=3600) => "",
                (<=1, _) => "ms",
                (<=60, _) => "s",
                (_, _) => "min",
            };
            string dWithTanni = DelaySeconds switch
            {
                < 1 => $"{DelaySeconds * 1000:F0}ms",
                < 60 => $"{DelaySeconds:F2}s",
                _ => $"{DelaySeconds / 60:F2}min",
            };
            return $"Delay {t}{tTanni}/{dWithTanni}";
        }
        #endregion
    }
}