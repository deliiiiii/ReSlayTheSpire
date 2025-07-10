using System;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        protected override async Task<EState> OnTickChild()
        {
            if (curNode == null)
            {
                RecursiveDo(OnResetState);
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var res = await curNode.Value.TickAsync();
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