using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    // [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
#if UNITY_EDITOR
        [Button]
        void ShowGraph()
        {
            BTEditorWindow.OpenGraph(this);
        }
#endif
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        public override string ToString()
        {
            return nameof(RootNode);
        }
    }
}