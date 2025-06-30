using System;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public abstract class ActionNode : NodeBase
    {
        [HideInEditorMode][HideInPlayMode]
        public Action OnEnter;
        [HideInEditorMode][HideInPlayMode]
        public Action<float> OnContinue;
        protected bool isFinished;
        public override EState OnTick(float dt)
        {
            if (!Tree.IsNodeRunning(this))
            {
                OnEnter?.Invoke();
                Tree.AddRunningNode(this);
            }
            OnContinue(dt);
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
        
        public override NodeBase AddChild(NodeBase child)
        {
            MyDebug.LogError("ActionNode cannot have children.");
            return this;
        }
    }

    

    
}