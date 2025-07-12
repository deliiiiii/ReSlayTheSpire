using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class LimitTimesNode : DecorateNode, IShowDetail
    {
        public int LimitTimes = 1;
        public int LimitTimer = 0;

        protected override void OnFail()
        {
            base.OnFail();
            LimitTimer = 0;
        }

        protected override EState OnTickChild(float dt)
        {
            if(LimitTimer >= LimitTimes)
                return EState.Succeeded;
            var ret = LastChild?.Tick(dt);
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