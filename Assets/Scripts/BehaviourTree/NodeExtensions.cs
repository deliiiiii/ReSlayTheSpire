using System;

namespace BehaviourTree
{
    public static class NodeExtensions
    {
        public static NodePlus<TParent, TThis, TNewChild> AddChildStay<TParent, TThis, TNewChild>
            (this INodePlus<TParent, TThis, NodeBase> tthis,TNewChild child)
            where TParent : NodeBase
            where TThis : CompositeNode
            where TNewChild : NodeBase
        {
            tthis.Node.AddChildStay(child);
            return new NodePlus<TParent, TThis, TNewChild>(tthis.Parent, tthis.Node);
        }
        
        public static NodePlus<TThis, TToChild, NodeBase> ToChild<TThis, TToChild>
            (this INodePlus<NodeBase, TThis, TToChild> tthis)
            where TThis : NodeBase
            where TToChild : NodeBase
        {
            return new NodePlus<TThis, TToChild, NodeBase>(tthis.Node, tthis.Child);
        }

        public static NodePlus<NodeBase, TParent, TThis> Back<TParent, TThis>
            (this INodePlus<TParent, TThis, NodeBase> tthis)
            where TParent : NodeBase
            where TThis : NodeBase
        {
            return new NodePlus<NodeBase, TParent, TThis>(tthis.Parent.Parent, tthis.Parent);
        }
        
        public static NodePlus<T1, T2, T3> SetChildName<T1, T2, T3>(this NodePlus<T1, T2, T3> node, string name)
            where T1 : NodeBase
            where T2 : NodeBase
            where T3 : NodeBase
        {
            node.Child.NodeName = name;
            return node;
        }
        
        public static NodePlus<T1, T2, T3> SetName<T1, T2, T3>(this NodePlus<T1, T2, T3> node, string name)
            where T1 : NodeBase
            where T2 : NodeBase
            where T3 : NodeBase
        {
            node.Node.NodeName = name;
            return node;
        }

        public static NodePlus<T1, T2, T3> SetGuard<T1, T2, T3>(this NodePlus<T1, T2, T3> node, Func<bool> condition)
            where T1 : NodeBase
            where T2 : NodeBase
            where T3 : NodeBase
        {
            node.Node.Guard = new Guard { Condition = condition };
            return node;
        }
        public static NodePlus<T1, T2, T3> RemoveGuard<T1, T2, T3>(this NodePlus<T1, T2, T3> node, Func<bool> condition)
            where T1 : NodeBase
            where T2 : NodeBase
            where T3 : NodeBase
        {
            node.Node.Guard = Guard.AlwaysTrue;
            return node;
        }
        
    }
}