using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        public override EState OnTick(float dt)
        {
            // 1变成0 0变成1
            return ~FirstChild?.OnTick(dt) ?? EState.Failed;
        }
        public override void OnFail()
        {
            FirstChild?.OnFail();
        }
    }

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