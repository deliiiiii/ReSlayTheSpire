using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public enum EEventK1
    {
        Vehicle,
        Electronic,
        Clothing
    }
    public static class BTEvent
    {
        static SerializableDictionary<EEventK1, List<string>> k1Tok2sDic => BTEventConfig.K1Tok2Dic;
        
        static Dictionary<EEventK1, Dictionary<string, Action>> eventsDic;
        static Dictionary<EEventK1, Dictionary<string, Action>> EventsDic
        {
            get
            {
                if (eventsDic != null)
                    return eventsDic;
                eventsDic = new Dictionary<EEventK1, Dictionary<string, Action>>();
                GetK1s().ForEach(k1 =>
                {
                    eventsDic.Add(k1, new Dictionary<string, Action>());
                    GetK2sByK1(k1).ForEach(k2 =>
                    {
                        eventsDic[k1].Add(k2, null);
                    });
                });
                return eventsDic;
            }
        }
        
        public static List<EEventK1> GetK1s() => k1Tok2sDic.Keys.ToList();
        public static List<string> GetK2sByK1(EEventK1 k1) => k1Tok2sDic[k1];
        public static void SendEvent((EEventK1, string) keys)
        {
            if (EventsDic == null)
                return;
            if (!EventsDic.ContainsKey(keys.Item1))
                return;
            if(!eventsDic[keys.Item1].ContainsKey(keys.Item2))
                return;
            EventsDic[keys.Item1][keys.Item2]?.Invoke();
        }
        public static void RegisterEvent((EEventK1, string) keys, Action action)
        {
            if (EventsDic == null)
                return;
            if (!EventsDic.ContainsKey(keys.Item1))
                return;
            if(!eventsDic[keys.Item1].ContainsKey(keys.Item2))
                return;
            EventsDic[keys.Item1][keys.Item2] += action;
        }
        public static void UnRegisterEvent((EEventK1, string) keys, Action action)
        {
            if (EventsDic == null)
                return;
            if (!EventsDic.ContainsKey(keys.Item1))
                return;
            if(!eventsDic[keys.Item1].ContainsKey(keys.Item2))
                return;
            EventsDic[keys.Item1][keys.Item2] -= action;
        }
    }
}