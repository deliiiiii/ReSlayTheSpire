using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public interface INodeBaseEditor<out T> where T : NodeBase
    {
        public T NodeBase { get; }
        void AddChildren();
    }
    public abstract class NodeBaseEditor<T> : Node, INodeBaseEditor<T> where T : NodeBase
    {
        public T NodeBase { get; protected set; }
        
        public void AddChildren()
        {
            //遍历节点的每个输出端口
            foreach (var outputPort in outputContainer.Q<Port>().connections)
            {
                //获取连接的输出端口对应的节点
                if (outputPort.input.node is not NodeBaseEditor<NodeBase> childNode)
                    continue;
                NodeBase.AddChild(childNode.NodeBase);
                //递归添加子节点
                childNode.AddChildren();
            }
        }
    }
}