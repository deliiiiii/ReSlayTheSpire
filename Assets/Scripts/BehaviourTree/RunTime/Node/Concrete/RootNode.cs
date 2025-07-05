using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    [Serializable]
    public class RootNode : NodeBase, IHasChild
    {
        public IEnumerable<ACDNode> ChildNodes => ChildNode != null ? new[] { ChildNode } : Enumerable.Empty<ACDNode>();
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
            ChildNode?.Tick(dt);
        }
    }
}