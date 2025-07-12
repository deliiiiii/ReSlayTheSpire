using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class LimitTimesNode : DecorateNode, IShowDetail
    {
        public int LimitTimes = 1;
        public int LimitTimer = 0;

        protected override void OnReset()
        {
            base.OnReset();
            LimitTimer = 0;
        }

        protected override EState Tick(float dt)
        {
            if (LimitTimer >= LimitTimes)
            {
                LastChild?.RecursiveDo(CallReset);
                return EState.Succeeded;
            }
            var ret = LastChild?.TickTemplate(dt);
            if(ret == null)
                return EState.Succeeded;
            if(ret == EState.Succeeded)
                LimitTimer++;
            return ret.Value;
        }
        public string GetDetail()
        {
            return $"Limited:{LimitTimer}/{LimitTimes}";
        }
    }
}