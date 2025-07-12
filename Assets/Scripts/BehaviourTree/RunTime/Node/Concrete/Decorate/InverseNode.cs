using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        protected override EState OnTickChild(float dt)
        {
            var ret = LastChild?.Tick(dt);
            if(ret == null)
                return EState.Succeeded;
            return ret.Value switch
            {
                EState.Succeeded => EState.Failed,
                EState.Failed => EState.Succeeded,
                EState.Running => EState.Running,
                _ => ret.Value
            };
        }
    }
}