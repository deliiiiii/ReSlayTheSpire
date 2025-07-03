using System;

namespace BehaviourTree
{
    [Serializable]
    public class GuardNodeAlwaysTrue : GuardNode
    {
        void OnEnable()
        {
            Condition = () => true;
        }
        public override string ToString()
        {
            return "Always True";
        }
    }
}