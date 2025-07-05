using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        public override EState OnTickChild(float dt)
        {
            var ret = FirstChild?.Tick(dt);
            if(ret == null)
                return EState.Failed;
            return ret == EState.Succeeded ? EState.Failed : EState.Succeeded;
        }
        public override void OnFail()
        {
            FirstChild?.OnFail();
        }
    }
}