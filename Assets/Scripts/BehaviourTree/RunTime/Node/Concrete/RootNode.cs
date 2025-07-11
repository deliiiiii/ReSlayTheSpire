using System;
using Sirenix.OdinInspector;
namespace BehaviourTree
{
    [Serializable]
    // [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
        [Button]
        void ShowGraph()
        {
            BTEditorWindow.OpenGraph(this);
        }
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        public override string ToString()
        {
            return nameof(RootNode);
        }
    }
}