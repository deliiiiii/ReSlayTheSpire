using System;
using UnityEngine;

namespace SubstanceP
{
    public class Transition
    {
        private float time, timer;
        private Func<float, float> ease;

        private bool pause, playing;
        public Action OnDrop, OnEnd, OnPause, OnResume;

        public event Action OnInit;
        public event Action<float> OnProcess;

        public Transition()
        {
            time = timer = 0;
            ease = Linear;
            pause = playing = false;
        }

        internal void Tick(float delta)
        {
            if (pause) return;
            float record = timer / time;
            if ((timer += delta) >= time)
            {
                OnEnd?.Invoke();
                timer = time;
                TransitionExecutor.Unregister(this);
            }
            Process(ease(timer / time) - ease(record));
        }

        protected virtual void Init() => OnInit?.Invoke();
        public void Init(Action init) => OnInit += init;

        protected virtual void Process(float delta) => OnProcess?.Invoke(delta);
        public void Process(Action<float> process) => OnProcess += process;

        public Transition Easing(Func<float, float> ease)
        {
            this.ease = ease;
            return this;
        }
        public Transition During(float time)
        {
            this.time = time;
            return this;
        }

        public Transition Play()
        {
            if (!playing)
            {
                playing = true;
                Init();
                TransitionExecutor.Register(this);
            }
            return this;
        }

        public void Drop()
        {
            TransitionExecutor.Unregister(this);
            OnDrop?.Invoke();
        }

        public void Pause()
        {
            pause = true;
            OnPause?.Invoke();
        }
        public void Resume()
        {
            if (!pause) return;
            pause = false;
            OnResume?.Invoke();
        }

        #region Easing
        public static float Step(float t) => Step(t, .5f);
        public static float Step(float t, float s) => (!(t < s)) ? 1 : 0;

        public static float Linear(float t) => t;

        public static float InSine(float t) => Mathf.Sin(MathF.PI / 2f * (t - 1f)) + 1f;

        public static float OutSine(float t) => Mathf.Sin(t * (MathF.PI / 2f));

        public static float InOutSine(float t) => (Mathf.Sin(MathF.PI * (t - .5f)) + 1f) * .5f;

        public static float InQuad(float t) => InPower(t, 2);

        public static float OutQuad(float t) => OutPower(t, 2);

        public static float InOutQuad(float t) => InOutPower(t, 2);

        public static float InCubic(float t) => InPower(t, 3);

        public static float OutCubic(float t) => OutPower(t, 3);

        public static float InOutCubic(float t) => InOutPower(t, 3);

        public static float InPower(float t, int power) => Mathf.Pow(t, power);

        public static float OutPower(float t, int power)
        {
            int num = ((power % 2 != 0) ? 1 : (-1));
            return num * (Mathf.Pow(t - 1f, power) + num);
        }

        public static float InOutPower(float t, int power)
        {
            t *= 2f;
            if (t < 1f) return InPower(t, power) * .5f;

            int num = ((power % 2 != 0) ? 1 : (-1));
            return num * .5f * (Mathf.Pow(t - 2f, power) + (num * 2));
        }

        public static float InBounce(float t) => 1f - OutBounce(1f - t);

        public static float OutBounce(float t)
        {
            if (t < .363636374f) return 7.5625f * t * t;

            if (t < .727272749f)
            {
                float num = (t -= .545454562f);
                return 7.5625f * num * t + .75f;
            }

            if (t < .909090936f)
            {
                float num2 = (t -= .8181818f);
                return 7.5625f * num2 * t + .9375f;
            }

            float num3 = (t -= 21f / 22f);
            return 7.5625f * num3 * t + 63f / 64f;
        }

        public static float InOutBounce(float t) => t < .5f ? InBounce(t * 2f) * .5f : OutBounce((t - .5f) * 2f) * .5f + .5f;

        public static float InElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            float num = .3f;
            float num2 = num / 4f;
            float num3 = Mathf.Pow(2f, 10f * (t -= 1f));
            return 0f - num3 * Mathf.Sin((t - num2) * (MathF.PI * 2f) / num);
        }

        public static float OutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            float num = .3f;
            float num2 = num / 4f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - num2) * (MathF.PI * 2f) / num) + 1f;
        }

        public static float InOutElastic(float t) => t < .5f ? InElastic(t * 2f) * .5f : OutElastic((t - .5f) * 2f) * .5f + .5f;

        public static float InBack(float t) => InBack(t, 1.70158f);

        public static float OutBack(float t) => OutBack(t, 1.70158f);

        public static float InOutBack(float t) => InOutBack(t, 1.70158f);

        public static float InBack(float t, float s) => t * t * ((s + 1f) * t - s);

        public static float OutBack(float t, float s) => 1f - InBack(1f - t, s);

        public static float InOutBack(float t, float s) => t < .5f ? InBack(t * 2f, s) * .5f : OutBack((t - .5f) * 2f, s) * .5f + .5f;

        public static float InCirc(float t) => 0f - (Mathf.Sqrt(1f - t * t) - 1f);

        public static float OutCirc(float t)
        {
            t -= 1f;
            return Mathf.Sqrt(1f - t * t);
        }

        public static float InOutCirc(float t)
        {
            t *= 2f;
            if (t < 1f) return -.5f * (Mathf.Sqrt(1f - t * t) - 1f);

            t -= 2f;
            return .5f * (Mathf.Sqrt(1f - t * t) + 1f);
        }
        #endregion
    }
}
