using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : ACDNode
    {
        [NonSerialized]
        public Action OnEnter;
        [NonSerialized]
        public Action<float> OnContinue;
        bool isRunning;
        protected bool IsFinished;
        
        public override EState OnTickChild(float dt)
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
        public override void OnFail()
        {
            isRunning = IsFinished = false;
        }

        public override ACDNode AddChild(ACDNode child)
        {
            // MyDebug.LogError("ActionNode cannot have children.");
            return this;
        }
    }

    

    
}