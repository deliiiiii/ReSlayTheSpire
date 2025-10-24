using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(RootNode), menuName = "BehaviourTree/" + nameof(RootNode))]
    public class RootNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.Single;
        [ReadOnly]
        public bool Running;
        [CanBeNull] BindDataUpdate b;
        protected override void OnEnable()
        {
            base.OnEnable();
            if(Position.x != 0 || Position.y != 0)
                return;
            Position = new Vector2(600, 200);
            Size = new Vector2(200, 200);
        }
        public void RestartTick()
        {
            StopTick();
            StartTick();
        }

        public void StartTick()
        {
            if (Running)
                return;
            Running = true;
            RecursiveDo(CallReset);
            // TODO obsolete
            // b = Binder.Update(dt => TickTemplate(dt));
        }
        public void StopTick()
        {
            Running = false;
            // TODO
            // b?.UnBind();
        }
        public override string ToString()
        {
            return nameof(RootNode);
        }
    }
}