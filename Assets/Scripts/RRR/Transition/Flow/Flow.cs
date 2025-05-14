using System;
using System.Collections.Generic;

namespace SubstanceP
{
    public class Flow
    {
        private readonly static Stack<Flow> pool = new();
        private Flow() { }
        public static Flow Create() => pool.TryPop(out var flow) ? flow : new();

        private readonly Queue<Action> actions = new();
        private bool waiting, running;

        public Flow Run()
        {
            if (!running)
            {
                running = true;
                Continue();
            }
            return this;
        }

        public Flow Then(Action action)
        {
            actions.Enqueue(action);
            return this;
        }

        public Flow Delay(Func<Transition> func) => Then(() =>
        {
            waiting = true;
            func().Play().OnEnd += Continue;
        });
        public Flow Delay(Transition transition) => Then(() =>
        {
            waiting = true;
            transition.Play().OnEnd += Continue;
        });
        public Flow Delay(float time) => Delay(new Transition().During(time));

        private void Continue()
        {
            waiting = false;
            while (actions.TryDequeue(out var action))
            {
                action.Invoke();
                if (waiting) return;
            }
            pool.Push(this);
            running = false;
        }
    }
}