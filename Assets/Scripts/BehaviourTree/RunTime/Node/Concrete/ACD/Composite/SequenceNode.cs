using System;

namespace BehaviourTree
{
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        public override EState OnTick(float dt)
        {
            if (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res == EState.Succeeded)
                {
                    curNode = curNode.Next;
                }
            }
            else
            {
                curNode = ChildLinkedList?.First;
            }
            
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running)
                {
                    return res;
                }
                if (res is EState.Failed)
                {
                    curNode = null;
                    return res;
                }
                curNode = curNode.Next;
            }
            return EState.Succeeded;
            
        }
        public override void OnFail()
        {
            var fNode = ChildLinkedList?.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }
    }
}