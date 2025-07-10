using System;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class InverseNode : DecorateNode
    {
        protected override async Task<EState> OnTickChild(float dt)
        {
            if(LastChild == null)
            {
                return EState.Succeeded;
            }
            var ret = await LastChild.TickAsync(dt);
            return ret switch
            {
                EState.Succeeded => EState.Failed,
                EState.Running => EState.Running,
                _ => EState.Succeeded
            };
        }
    }
}