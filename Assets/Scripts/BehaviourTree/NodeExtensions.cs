namespace BehaviourTree
{
    public static class NodeExtensions
    {
        // public static CompositeNodePlus<TThis, TParent, TChild> AddChildStayPlus<TThis, TParent, TChild>(this TThis tthis,
        //     TChild child)
        //     where TThis : CompositeNode
        //     where TParent : NodeBase
        //     where TChild : NodeBase
        // {
        //     tthis.AddChildStay(child);
        //     return new CompositeNodePlus<TThis, TParent, TChild>(tthis, tthis.Parent, child);
        // }
        
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
            return new NodePlus<TThis, TToChild, NodeBase>(tthis.Node, tthis.GetChild() as TToChild);
        }

        public static NodePlus<NodeBase, TParent, TThis> Back<TParent, TThis>
            (this INodePlus<TParent, TThis, NodeBase> tthis)
            where TParent : NodeBase
            where TThis : NodeBase
        {
            return new NodePlus<NodeBase, TParent, TThis>(tthis.Parent.Parent, tthis.Parent);
        }
        
        
        
        
        // public static NodePlus<CompositeNode, TParent, TPlusChild> AddChildStayPlus<TThis, TParent, TPlusChild>(this TThis tthis,TPlusChild child)
        //     where TThis : NodePlus<CompositeNode, TParent, TPlusChild>
        //     where TParent : NodeBase
        //     where TPlusChild : NodeBase
        // {
        //     tthis.AddChildStay(child);
        //     return new NodePlus<CompositeNode, TParent, TPlusChild>(tthis, tthis.Parent, child);
        // }
        
        //
        //
        //
        // public static TThis BackPlus<TThis, TChild>(this CompositeNodePlus<TThis, TChild> thiss)
        //     where TThis : CompositeNode
        //     where TChild : NodeBase
        // {
        //     return thiss.Parent;
        // }
        
        // public static CompositeNodePlus<TParent, TChild> SetName<TParent, TChild>(this TParent parent, string name)
        //     where TParent : CompositeNode
        //     where TChild : NodeBase
        // {
        //     parent.Name = name;
        //     return new CompositeNodePlus<TParent, TChild>(parent, null);
        // }
        
        
        
        public static T SetChildName<T>(this T tthis, string childName)
        where T : NodeBase
        {
            tthis.GetChild().Name = childName;
            return tthis;
        }
        
    }
}