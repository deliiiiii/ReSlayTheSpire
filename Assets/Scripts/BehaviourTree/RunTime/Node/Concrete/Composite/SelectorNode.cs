using System;

namespace BehaviourTree
{
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        protected override EState OnTickChild(float dt)
        {
            if(curNode == null)
            {
                RecursiveDo(MyReset);
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running)
                {
                    return res;
                }
                if (res is EState.Succeeded)
                {
                    curNode = null;
                    return res;
                }
                curNode = curNode.Next;
            }
            return EState.Failed;
        }
    }
}