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
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        public override string ToString()
        {
            return nameof(RootNode);
        }

        public override EState Tick(float dt)
        {
            return State.Value = LastChild?.Tick(dt) ?? EState.Failed;
        }
    }
}