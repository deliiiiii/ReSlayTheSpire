using System;

namespace BehaviourTree
{
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        public override EState OnTickChild(float dt)
        {
            if (curNode == null)
            {
                OnResetState();
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running)
                {
                    return res;
                }
                if (res is EState.Failed)
                {
                    curNode = null;
                    return res;
                }
                curNode = curNode.Next;
            }
            return EState.Succeeded;
            
        }
        
    }
}