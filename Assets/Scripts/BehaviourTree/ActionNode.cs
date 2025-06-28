using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        public string Name => ToString();
        [HideInEditorMode][HideInPlayMode]
        public Action Act;

        public override bool OnTick(float dt)
        {
            Act();
            return true;
        }
    }

    [Serializable]
    public class ActionNodeDebug : ActionNode
    {
        public string Content;

        public ActionNodeDebug(string content)
        {
            Content = content;
            Act = () =>
            {
                MyDebug.Log(Content, LogType.Tick);
            };
        }

        public override string ToString()
        {
            return $"{Content}";
        }
    }

    [Serializable]
    public class ActionNodeDelay : ActionNode
    {
        public float DelaySeconds;
        float timer;
        
        public ActionNodeDelay(float delaySeconds)
        {
            DelaySeconds = delaySeconds;
        }
        public override bool OnTick(float dt)
        {
            timer += dt;
            return timer >= DelaySeconds;
        }
    }
}