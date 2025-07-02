using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public abstract class GuardNode : NodeBase
    {
        [NonSerialized]
        public Func<bool> Condition;
    }
}