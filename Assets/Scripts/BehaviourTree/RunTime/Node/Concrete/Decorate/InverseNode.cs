using System;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        protected override async Task<EState> OnTickChild()
        {
            if(LastChild == null)
            {
                return EState.Succeeded;
            }
            var ret = await LastChild.TickAsync();
            return ret switch
            {
                EState.Succeeded => EState.Failed,
                _ => EState.Succeeded
            };
        }
    }
}