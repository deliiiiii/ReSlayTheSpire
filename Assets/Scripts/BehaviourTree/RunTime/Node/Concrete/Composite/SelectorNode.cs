using System;

namespace BehaviourTree
{
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        protected override EState OnTickChild(float dt)
        {
            if(curNodeId == -1)
            {
                RecursiveDo(OnResetState);
                curNodeId = 0;
            }
            
            while (curNodeId != -1 && curNodeId < ChildList.Count)
            {
                var res = ChildList[curNodeId].Tick(dt);
                if (res is EState.Running)
                {
                    return res;
                }
                if (res is EState.Succeeded)
                {
                    curNodeId = -1;
                    return res;
                }

                curNodeId++;
            }
            curNodeId = -1;
            return EState.Failed;
        }
    }
}