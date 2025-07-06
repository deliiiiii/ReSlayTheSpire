using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
        public ACDNode ChildNode;
        public override string ToString()
        {
            return nameof(RootNode);
        }

        public void Tick(float dt)
        {
            State.Value = ChildNode?.Tick(dt) ?? EState.Failed;
        }
    }
}