using System;

namespace BehaviourTree
{
    [Serializable]
    public class Tree
    {
        public string Name = "New Tree";
        public SequenceNode Root;
        // public NodeBase CurrentNode;

        public SequenceNode Create()
        {
            return Root = new SequenceNode();
        }

        public void Tick(float dt)
        {
            Root.Tick(dt);
        }
    }
}