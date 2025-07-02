using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [ShowInInspector]
        public string Name => ToString();
        [HideInInspector]
        public Func<bool> Condition;
    }
}