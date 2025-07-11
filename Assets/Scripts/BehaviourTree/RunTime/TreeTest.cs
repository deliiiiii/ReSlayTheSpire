using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

namespace BehaviourTree
{
    public class TreeTest : SerializedMonoBehaviour
    {
        // [NonSerialized, OdinSerialize]
        public RootNode Root;
        public int TarFrameRate = 10;
        public bool ShowStartTick = true;
        void Start()
        {
            Application.targetFrameRate = TarFrameRate;
            Binder.Update(Tick);
        }

        void Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            Root.Tick(dt);
        }
    }
}