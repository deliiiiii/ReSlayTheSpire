using System;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [NonSerialized]
        public Func<bool> Condition;
#if UNITY_EDITOR
        public override void OnSave()
        {
        }
#endif
    }
    
}