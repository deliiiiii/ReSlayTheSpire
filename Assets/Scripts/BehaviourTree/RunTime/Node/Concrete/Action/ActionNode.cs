using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        
        public Action OnEnter;
        public Action<float> OnContinue;
        bool isRunning;
        protected bool IsFinished;

        public ActionNode()
        {
            OnContinue = _ => IsFinished = true;
            Binder.From(State).To(s =>
            {
                if(s == EState.Failed)
                    isRunning = IsFinished = false;
            });
        }

        protected override EState OnTickChild(float dt)
        {
            if (!isRunning)
            {
                OnEnter?.Invoke();
                isRunning = true;
            }
            OnContinue?.Invoke(dt);
            if (IsFinished)
            {
                isRunning = IsFinished = false;
                return EState.Succeeded;
            }
            return EState.Running;
        }
    }

    

    
}