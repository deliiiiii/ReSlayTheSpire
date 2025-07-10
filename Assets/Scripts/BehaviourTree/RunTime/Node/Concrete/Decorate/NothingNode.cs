using System;
using System.Threading.Tasks;

namespace BehaviourTree
{
    [Serializable]
    public class NothingNode : DecorateNode
    {
        // protected override async Task<EState> OnTickChild(float dt)
        // {
        //     var ret = await LastChild?.TickAsync(dt);
        //     return ret ?? EState.Succeeded;
        // }
    }
}