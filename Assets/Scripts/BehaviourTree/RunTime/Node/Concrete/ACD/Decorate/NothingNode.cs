using System;

namespace BehaviourTree
{
    [Serializable]
    public class NothingNode : DecorateNode
    {
        public override EState OnTick(float dt)
        {
            // 什么都不做
            return EState.Succeeded;
        }
        public override void OnFail()
        {
            FirstChild?.OnFail();
        }
    }
}