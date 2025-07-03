using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviourTree
{
    [Serializable]
    public class RootNode : NodeBase
    {
        public ACDNode ChildNode;
        public override string ToString()
        {
            return nameof(RootNode);
        }
        
#if UNITY_EDITOR
        public override void OnSave()
        {
            AssetDatabase.AddObjectToAsset(ChildNode, this);
            ChildNode?.OnSave();
        }
#endif

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