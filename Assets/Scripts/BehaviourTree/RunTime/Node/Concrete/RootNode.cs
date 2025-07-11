using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
        public void OnEnable()
        {
            if(Position.x != 0 || Position.y != 0)
                return;
            Position = new Vector2(600, 200);
            Size = new Vector2(200, 200);
        }

        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        public override string ToString()
        {
            return nameof(RootNode);
        }
    }
}