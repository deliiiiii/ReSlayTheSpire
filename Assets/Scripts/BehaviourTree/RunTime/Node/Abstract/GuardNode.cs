using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNode : NodeBase
    {
        [ShowInInspector]
        public string Name => ToString();
        // [HideInInspector]
        public Func<bool> Condition;
        public static GuardNode AlwaysTrue = new GuardNodeAlwaysTrue();
        public override EState OnTick(float dt)
        {
            throw new NotImplementedException();
        }

        public override void OnFail()
        {
            throw new NotImplementedException();
        }

        public override NodeBase AddChild(NodeBase child)
        {
            throw new NotImplementedException();
        }
    }
}