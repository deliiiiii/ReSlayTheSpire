using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public static class BTEvent
    {
        static SerializableDictionary<EEvent, Action> eventDic; 
        public static void SendEvent(EEvent eventName)
        {
            eventDic?[eventName]?.Invoke();
        }
        public static void RegisterEvent(EEvent eType, Action action)
        {
            eventDic ??= new SerializableDictionary<EEvent, Action>();
            if (!eventDic.TryAdd(eType, action))
            {
                eventDic[eType] += action;
            }
        }

        public static void UnRegisterEvent(EEvent eType, Action action)
        {
            if (eventDic == null || !eventDic.ContainsKey(eType))
                return;
            eventDic[eType] -= action;
        }
    }
}