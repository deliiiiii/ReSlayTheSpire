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
                return EState.Succeeded;
            return ret == EState.Succeeded ? EState.Failed : 
                ret == EState.Running ? EState.Running :
                EState.Succeeded;
        }
        public override void OnFail()
        {
            FirstChild?.OnFail();
        }
    }
}