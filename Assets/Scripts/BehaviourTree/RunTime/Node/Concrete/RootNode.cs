using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        
        [InlineEditor]
        public Blackboard Blackboard;
        
        public override string ToString()
        {
            return nameof(RootNode);
        }
        
        public void OnRefreshTreeEnd()
        {
            RecursiveDo(x => SetBlackboard(x, Blackboard));
        }
    }
}