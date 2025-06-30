using System;

namespace BehaviourTree
{
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        public SequenceNode() { }
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
                curNode = childList.First;
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
            var fNode = childList.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }
    }
    
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        public SelectorNode() { }
        public override EState OnTick(float dt)
        {
            // curNode ??= childList.First;
            curNode = childList.First;
            while (curNode != null)
            {
                var res = curNode.Value.Tick(dt);
                if (res is EState.Running or EState.Succeeded)
                    return res;
                curNode = curNode.Next;
            }
            return EState.Failed;
        }
        public override void OnFail()
        {
            var fNode = childList.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }
    }
}