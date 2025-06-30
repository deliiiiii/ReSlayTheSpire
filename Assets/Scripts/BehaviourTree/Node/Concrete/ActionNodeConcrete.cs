using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeConcrete : ActionNode
    {
        
        public string Content;
        public ActionNodeConcrete() { }
        public ActionNodeConcrete(string content)
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
        public ActionNodeDelay(){}
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
        readonly T tarValue;
        public ActionNodeSet(){}
        public ActionNodeSet(T tarValue, Action<T> setter)
        {
            this.tarValue = tarValue;
            OnContinue = _ =>
            {
                setter(this.tarValue);
                isFinished = true;
            };
        }
        public override string ToString()
        {
            return $"Set to {tarValue}";
        }
    }
}