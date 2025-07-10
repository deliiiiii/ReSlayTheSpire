using System;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        protected override async Task<EState> OnTickChild(CancellationTokenSource cts)
        {
            if(LastChild == null)
            {
                return EState.Succeeded;
            }
            var ret = await LastChild.TickAsync(cts);
            return ret switch
            {
                EState.Succeeded => EState.Failed,
                _ => EState.Succeeded
            };
        }
    }
}