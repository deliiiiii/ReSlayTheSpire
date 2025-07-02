using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class ActionNode : ACDNode
    {
        [HideInInspector]
        public Action OnEnter;
        [HideInInspector]
        public Action<float> OnContinue;
        protected bool isFinished;
        
        public override EState OnTick(float dt)
        {
            if (!Tree.IsNodeRunning(this))
            {
                OnEnter?.Invoke();
                Tree.AddRunningNode(this);
            }
            OnContinue?.Invoke(dt);
            if (isFinished)
            {
                Tree.RemoveRunningNode(this);
                return EState.Succeeded;
            }
            return EState.Running;
        }
        public override void OnFail()
        {
            isFinished = false;
            Tree.RemoveRunningNode(this);
        }

        public override ACDNode AddChild(ACDNode child)
        {
            // MyDebug.LogError("ActionNode cannot have children.");
            return this;
        }
    }

    

    
}