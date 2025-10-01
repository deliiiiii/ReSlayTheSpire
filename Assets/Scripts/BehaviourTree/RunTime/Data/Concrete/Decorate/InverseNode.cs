using System;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        protected override EState Tick(float dt)
        {
            var ret = LastChild?.TickTemplate(dt);
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