using System;

namespace BehaviourTree
{
    [Serializable]
    public class NothingNode : DecorateNode
    {
        public override EState OnTickChild(float dt)
        {
            var ret = FirstChild?.Tick(dt);
            return ret ?? EState.Succeeded;
        }
    }
}