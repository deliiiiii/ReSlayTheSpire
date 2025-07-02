using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    public class RootNode : NodeBase
    {
        public ACDNode ChildNode;
        public override string ToString()
        {
            return nameof(RootNode);
        }
        public void Tick(float dt)
        {
            // if (RunningNodeSet != null && RunningNodeSet.Count > 0)
            // {
            //     RunningNodeSet.ToArray()[^1].Tick(dt);
            //     return;
            // }
            ChildNode.Tick(dt);
        }
    }
}