using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class ActionNode : NodeBase
    {
        public string Name => ToString();
        [HideInEditorMode][HideInPlayMode]
        public Action OnEnter;
        [HideInEditorMode][HideInPlayMode]
        public Action<float> OnContinue;
        protected bool isFinished;

        public override NodeBase GetChild()
        {
            return null;
        }

        public override bool OnTick(float dt)
        {
            if (!Tree.IsNodeRunning(this))
            {
                OnEnter?.Invoke();
                Tree.AddRunningNode(this);
            }
            if(isFinished)
                return true;
            OnContinue(dt);
            return isFinished;
        }
        public override void OnFail()
        {
            isFinished = false;
            Tree.RemoveRunningNode(this);
        }
    }

    [Serializable]
    public class ActionNodeDebug : ActionNode
    {
        public string Content;

        public ActionNodeDebug(string content)
        {
            Content = content;
            OnContinue = _ =>
            {
                MyDebug.Log(Content, LogType.Tick);
                isFinished = true;
            };
        }

        public override string ToString()
        {
            return $"{Content}";
        }
    }

    [Serializable]
    public class ActionNodeDelay : ActionNode
    {
        public float DelaySeconds;
        [ShowInInspector][ReadOnly]
        float timer;
        
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
        public T TarValue;
        public Action<T> Setter;

        public ActionNodeSet(T tarValue, Action<T> setter)
        {
            TarValue = tarValue;
            Setter = setter;
            OnContinue = _ =>
            {
                Setter(TarValue);
                isFinished = true;
            };
        }

        public override string ToString()
        {
            return $"Set to {TarValue}";
        }
    }
}