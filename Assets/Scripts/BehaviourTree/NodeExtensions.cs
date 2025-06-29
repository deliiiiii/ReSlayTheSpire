namespace BehaviourTree
{
    public static class NodeExtensions
    {
        public static T ToChild<T>(this CompositeNode node) where T : NodeBase
        {
            return node.childList?.Last?.Value as T;
        }

        // public static CompositeNodePlus<TThis, TParent, TChild> AddChildStayPlus<TThis, TParent, TChild>(this TThis tthis,
        //     TChild child)
        //     where TThis : CompositeNode
        //     where TParent : NodeBase
        //     where TChild : NodeBase
        // {
        //     tthis.AddChildStay(child);
        //     return new CompositeNodePlus<TThis, TParent, TChild>(tthis, tthis.Parent, child);
        // }
        
        public static NodePlus<NullNode, CompositeNode, TChild> AddChildStayPlus<TThis, TChild>(this TThis tthis,TChild child)
            where TThis : INodePlus<NullNode, CompositeNode, NullNode>
            where TChild : NodeBase
        {
            tthis.Node.AddChildStay(child);
            return new NodePlus<NullNode, CompositeNode, TChild>(tthis.Parent, tthis.Node);
        }
        
        
        
        public static NodePlus<CompositeNode, NodeBase, NodeBase> ToChild<TThis>(this TThis tthis)
            // where TParent : NodeBase
            where TThis : INodePlus<NodeBase, CompositeNode, NodeBase>
            // where TChild : NodeBase
        {
            return new NodePlus<CompositeNode, NodeBase, NodeBase>(tthis.Node, tthis.GetChild());
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
        
        
        
        // public static CompositeNodePlus<TParent, TChild> SetChildNamePlus<TParent, TChild>(this CompositeNodePlus<TParent, TChild> parent, string childName)
        //     where TParent : CompositeNode
        //     where TChild : NodeBase
        // {
        //     var child = parent.childList.Last.Value as TChild;
        //     child!.Name = childName;
        //     return new CompositeNodePlus<TParent, TChild>(parent, child);
        // }
        
    }
}