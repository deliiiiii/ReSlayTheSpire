using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class Tree
    {
        public string Name = "New Tree";
        [ShowInInspector]
        public NodeBase Root;
        // public NodeBase CurrentNode;
        [ShowInInspector]
        HashSet<ActionNode> RunningNodeSet;
        public void AddRunningNode(ActionNode node)
        {
            RunningNodeSet ??= new HashSet<ActionNode>();
            RunningNodeSet.Add(node);
        }
        public void RemoveRunningNode(ActionNode node)
        {
            RunningNodeSet?.Remove(node);
        }
        public bool IsNodeRunning(ActionNode node)
        {
            return RunningNodeSet != null && RunningNodeSet.Contains(node);
        }

        public T Create<T>() where T : NodeBase, new()
        {
            Root = new T
            {
                Tree = this
            };
            return Root as T;
        }

        public void Tick(float dt)
        {
            Root.Tick(dt);
        }
    }
}