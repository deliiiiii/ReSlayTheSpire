using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [Serializable]
    public class DecorateNode : ACDNode
    {
        public override EState OnTickChild(float dt)
        {
            throw new NotImplementedException();
        }

        public override void OnFail()
        {
            FirstChild?.OnFail();
        }

        public override ACDNode AddChild(ACDNode child)
        {
            if (FirstChild != null)
            {
                MyDebug.LogError("InverseNode can only have one child.");
                return this;
            }
            ChildList ??= new List<ACDNode>();
            ChildList.Add(child);
            return this;
        }
    }
    
    
}