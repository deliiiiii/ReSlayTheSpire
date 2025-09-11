using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class NodeWithEvent
    {
        public EEventK1 EventK1;
        [ValueDropdown(nameof(GetK2sByK1))]
        public string EventK2;
        
        List<string> GetK2sByK1()
        {
            var k2s = BTEvent.GetK2sByK1(EventK1).ToList();
            if (k2s.Count != 0 && !k2s.Contains(EventK2))
            {
                EventK2 = k2s[0];
            }
            return k2s;
        }
    }
}