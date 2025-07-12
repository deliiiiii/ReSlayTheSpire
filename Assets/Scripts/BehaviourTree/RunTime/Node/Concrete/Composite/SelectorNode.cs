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
        /// <summary>
        /// 根据list随机数的权重,随机选择子节点，且失败后不会尝试下一个节点
        /// </summary>
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
        
        protected override EState Tick(float dt)
        {
            if (State.Value is not EState.Running)
            {
                RecursiveDo(CallReset);
                curNode = IsRandom ? 
                    ChildLinkedList?.At(FixedSelections.RandomIndexWeighted()) :
                    ChildLinkedList?.First;
            }
            if (IsRandom)
            {
                return curNode?.Value.TickTemplate(dt) ?? EState.Failed;
            }
            while (curNode != null)
            {
                var ret = curNode.Value.TickTemplate(dt);
                if (ret is EState.Running or EState.Succeeded)
                {
                    return ret;
                }
                curNode = curNode.Next;
            }
            return EState.Failed;
        }

        public string GetDetail()
        {
            if (!IsRandom)
                return string.Empty;
            return $"Ran:[{string.Join(",", FixedSelections)}]";
        }
    }
}