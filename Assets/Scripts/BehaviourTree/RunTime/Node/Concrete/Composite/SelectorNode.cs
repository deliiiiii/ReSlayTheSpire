using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class SelectorNode : CompositeNode, IShowDetail
    {
        public bool IsRandom;
        [ShowIf(nameof(IsRandom))]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public List<int> FixedSelections = new ();
        public override void OnRefreshTreeEnd()
        {
            base.OnRefreshTreeEnd();
            while(FixedSelections.Count < ChildCount)
                FixedSelections.Add(0);
            while (FixedSelections.Count > ChildCount)
                FixedSelections.RemoveAt(FixedSelections.Count - 1);
        }
        protected override EState OnTickChild(float dt)
        {
            if(curNode == null)
            {
                RecursiveDo(MyReset);
                if (IsRandom)
                {
                    // 根据list随机数的权重,随机选择子节点，且失败后不会尝试下一个节点
                    if (ChildLinkedList == null || ChildLinkedList.Count == 0)
                    {
                        return EState.Failed;
                    }
                    curNode = ChildLinkedList.At(FixedSelections.RandomIndexWeighted());
                }
                else
                {
                    curNode = ChildLinkedList?.First;
                }
            }

            if (IsRandom)
            {
                var ret = curNode.Value.Tick(dt);
                if (ret is not EState.Running)
                {
                    curNode = null;
                }
                return ret;
            }
            while (curNode != null)
            {
                var ret = curNode.Value.Tick(dt);
                if (ret is EState.Running)
                {
                    return ret;
                }
                if (ret is EState.Succeeded)
                {
                    curNode = null;
                    return ret;
                }
                curNode = curNode.Next;
            }
            return EState.Failed;
        }

        public string GetDetail()
        {
            return $"Ran:[{string.Join(",", FixedSelections)}]";
        }
    }
}