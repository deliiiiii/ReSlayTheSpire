using System;

namespace BehaviourTree
{
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        public override EState OnTick(float dt)
        {
            // curNode ??= childList.First;
            curNode = ChildList.First;
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
            var fNode = ChildList.First;
            while (fNode != null)
            {
                fNode.Value.OnFail();
                fNode = fNode.Next;
            }
        }
    }
}