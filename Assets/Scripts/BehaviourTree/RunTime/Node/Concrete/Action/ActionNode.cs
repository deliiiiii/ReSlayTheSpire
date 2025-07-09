using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        
        [NonSerialized]
        public Action OnEnter;
        [NonSerialized]
        public Action<float> OnContinue;
        bool isRunning;
        protected bool IsFinished;

        void OnEnable()
        {
            Binder.From(State).To(s =>
            {
                if(s == EState.Failed)
                    isRunning = IsFinished = false;
            });
        }


        protected override async Task<EState> OnTickChild(float dt)
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