using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class Tree
    {
        public string Name = "New Tree";
        [ShowInInspector]
        public NodeBase Root;
        // [ShowInInspector]
        // public NodeBase CurRunningNode;
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

        public NodePlus<NodeBase, T, NodeBase> Create<T>() where T : CompositeNode, new()
        {
            var t = new T
            {
                Tree = this
            };
            Root = t;
            return new NodePlus<NodeBase, T, NodeBase>(t);
        }

        public void Tick(float dt)
        {
            // if (RunningNodeSet != null && RunningNodeSet.Count > 0)
            // {
            //     RunningNodeSet.ToArray()[^1].Tick(dt);
            //     return;
            // }
            Root.Tick(dt);
        }
    }
}