using System;

namespace BehaviourTree
{
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        protected override EState Tick(float dt)
        {
            if (State.Value is not EState.Running)
            {
                RecursiveDo(CallReset);
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var ret = curNode.Value.TickTemplate(dt);
                if (ret is EState.Running or EState.Failed)
                {
                    return ret;
                }
                curNode = curNode.Next;
            }
            return EState.Succeeded;
            
        }
        
    }
}