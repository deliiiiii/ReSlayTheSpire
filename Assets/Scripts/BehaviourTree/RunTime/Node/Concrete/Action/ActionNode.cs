using System;
using JetBrains.Annotations;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        [CanBeNull] protected virtual Action OnEnter => null;
        [NotNull] protected virtual Action<float> OnContinue => _ => IsFinished = true;

        [HideInInspector]
        public bool IsRunning;
        [HideInInspector]
        public bool IsFinished;
        protected override void OnFail()
        {
            IsRunning = IsFinished = false;
        }
        protected override EState OnTickChild(float dt)
        {
            if (!IsRunning)
            {
                OnEnter?.Invoke();
                IsRunning = true;
            }
            OnContinue.Invoke(dt);
            if (IsFinished)
            {
                IsRunning = IsFinished = false;
                return EState.Succeeded;
            }
            return EState.Running;
        }
    }

    

    
}