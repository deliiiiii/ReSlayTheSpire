using System;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        protected override async Task<EState> OnTickChild(float dt)
        {
            if(curNode == null)
            {
                RecursiveDo(OnResetState);
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var res = await curNode.Value.TickAsync(dt);
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