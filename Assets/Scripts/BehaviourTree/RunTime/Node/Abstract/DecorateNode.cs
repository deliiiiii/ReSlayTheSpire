using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public abstract class DecorateNode : NodeBase
    {
        protected NodeBase curChild => ChildList?.Count > 0 ? ChildList[0] : null;
        public override NodeBase AddChild(NodeBase child)
        {
            if (curChild != null)
            {
                MyDebug.LogError("InverseNode can only have one child.");
                return this;
            }
            ChildList ??= new List<NodeBase>{child};
            return this;
        }
    }
    
    
}