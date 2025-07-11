﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNodeEvent : ActionNode, IShowDetail
    {
        public EEventK1 EventK1Type;
        [ValueDropdown(nameof(GetK2sByK1))]
        public string EventK2;

        protected override void OnEnable()
        {
            base.OnEnable();
            OnDelayEnd += () => BTEvent.SendEvent((EventK1Type, EventK2));
        }
        List<string> GetK2sByK1()
        {
            var k2s = BTEvent.GetK2sByK1(EventK1Type).ToList();
            if (k2s.Count != 0 && !k2s.Contains(EventK2))
            {
                EventK2 = k2s[0];
            }
            return k2s;
        }
        public new string GetDetail()
        {
            return $"{base.GetDetail()}{EventK1Type.ToString()}::{EventK2}";
        }
    }
}