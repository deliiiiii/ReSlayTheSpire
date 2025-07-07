using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BehaviourTree
{
    public static class INodeBaseEditorExt
    {
        [CanBeNull]
        public static IEnumerable<INodeBaseEditor<NodeBase>> ChildEditors(this INodeBaseEditor<NodeBase> n) 
            => n.OutputChildPorts?.connections?
                .Where(port => port.input.node is INodeBaseEditor<NodeBase>)
                .Select(port => port.input.node as INodeBaseEditor<NodeBase>);

        [CanBeNull]
        public static INodeBaseEditor<GuardNode> GuardingEditor(this INodeBaseEditor<NodeBase> n)
            => n.InputGuardingPort?.connections?
                .Where(port => port.output.node is INodeBaseEditor<GuardNode>)
                .Select(port => port.output.node as INodeBaseEditor<GuardNode>)
                .FirstOrDefault();
        
        [CanBeNull]
        public static GuardNode GuardingNode(this INodeBaseEditor<NodeBase> n)
            => n.GuardingEditor()?.NodeBase;
    }
}