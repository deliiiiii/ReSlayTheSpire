using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class TreeHolder : MonoBehaviour
    {
        public RootNode Root;
        public bool RunOnStart;
        public bool EnableEvents = true;
        [HelpBox("Reset ↓", HelpBoxType.Warning)]
        public bool IWannaRefillTheWholeDiccccc;
        [HelpBox("Reset ↑", HelpBoxType.Warning)]
        public bool IWannaRefillTheEventType;
        public EEventK1 EventType;
        public SerializableDictionary<EEventK1, SerializableDictionary<string, List<UnityEvent>>> TypeToEvents;
        
        void OnValidate()
        {
            if (Application.isPlaying)
                return;
            if (IWannaRefillTheWholeDiccccc)
            {
                IWannaRefillTheWholeDiccccc = false;
                MyDebug.Log("You Have Refilled The Whole Dic.");
                BTEvent.GetK1s().ForEach(k1 =>
                {
                    TypeToEvents.Add(k1, new SerializableDictionary<string, List<UnityEvent>>());
                    BTEvent.GetK2sByK1(k1).ForEach(k2 =>
                    {
                        TypeToEvents[k1].Add(k2, new List<UnityEvent>());
                    });
                });
            }
            if (IWannaRefillTheEventType)
            {
                IWannaRefillTheEventType = false;
                MyDebug.Log($"You Have Refilled The Event Type: {EventType}");
                BTEvent.GetK2sByK1(EventType).ForEach(str =>
                {
                    TypeToEvents[EventType].TryAdd(str, new List<UnityEvent>());
                });
            }
        }
        void Start()
        {
            if (EnableEvents)
                RegisterAllEvents();
            if (RunOnStart)
                Root.RestartTick();
        }

        void RegisterAllEvents()
        {
            TypeToEvents?.Keys.ForEach(eventType =>
            {
                TypeToEvents[eventType].Keys.ForEach(str =>
                {
                    TypeToEvents[eventType][str].ForEach(e => BTEvent.RegisterEvent((eventType, str), e.Invoke));
                });
            });
        }
    }
}