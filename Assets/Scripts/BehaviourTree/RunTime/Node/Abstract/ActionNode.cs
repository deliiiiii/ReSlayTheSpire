using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class ActionNode : ACDNode
    {
        [NonSerialized]
        public Action OnEnter;
        [NonSerialized]
        public Action<float> OnContinue;
        bool isRunning;
        protected bool isFinished;
        
        public override EState OnTick(float dt)
        {
            if (!isRunning)
            {
                OnEnter?.Invoke();
                isRunning = true;
            }
            OnContinue?.Invoke(dt);
            if (isFinished)
            {
                isRunning = isFinished = false;
                return EState.Succeeded;
            }
            return EState.Running;
        }
        public override void OnFail()
        {
            isRunning = isFinished = false;
        }

        public override ACDNode AddChild(ACDNode child)
        {
            // MyDebug.LogError("ActionNode cannot have children.");
            return this;
        }
    }

    

    
}