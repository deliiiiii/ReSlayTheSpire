using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [HideInInspector]
        public Func<bool> Condition;
    }
}