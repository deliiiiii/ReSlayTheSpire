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
        public int TarFrameRate = 10;
        public bool ShowStartTick = true;
        public bool RunOnStart;
        public bool EnableEvents = true;
        public SerializableDictionary<EEvent, List<UnityEvent>> EventDic;
        BindDataUpdate b;
        void Start()
        {
            Application.targetFrameRate = TarFrameRate;
            if (EnableEvents)
                RegisterAllEvents();
            if (RunOnStart)
                b = Binder.Update(Tick);
        }

        void Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            Root.Tick(dt);
        }

        void RegisterAllEvents()
        {
            EventDic?.ForEach(kvp =>
            {
                kvp.Value.ForEach(e =>
                {
                    BTEvent.RegisterEvent(kvp.Key, e.Invoke);
                });
            });
        }
    }
}