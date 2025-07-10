using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        
        [NonSerialized]
        public Action OnEnter;

        [NonSerialized] public Action<float> OnContinue;
        bool isRunning;
        protected bool IsFinished;

        protected void OnEnable()
        {
            OnContinue = _ => IsFinished = true;
            // Binder.From(State).To(s =>
            // {
            //     if(s == EState.Failed)
            //         isRunning = IsFinished = false;
            // });
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