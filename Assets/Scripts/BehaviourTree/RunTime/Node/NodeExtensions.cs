using System;

namespace BehaviourTree
{
    public static class NodeExtensions
    {
        public static NodeBase SetName(this NodeBase node, string name)
        {
            node.NodeName = name;
            return node;
        }
        public static NodeBase SetChildName(this NodeBase node, string name)
        {
            if (node.ToChild() == null)
            {
                MyDebug.LogError($"Node \"{node.NodeName}\" has no child, cannot set child name.");
                return node;
            }
            node.ToChild().NodeName = name;
            return node;
        }
        public static NodeBase SetGuard(this NodeBase node, Func<bool> condition)
        {
            node.Guard = new Guard { Condition = condition };
            return node;
        }
        public static NodeBase RemoveGuard(this NodeBase node)
        {
            node.Guard = Guard.AlwaysTrue;
            return node;
        }
        
    }
}