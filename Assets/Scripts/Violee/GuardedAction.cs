using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Violee
{
    public class GuardedAction
    {
        readonly Action act;
        public event Func<bool> Guard;
        public GuardedAction(Action action)
        {
            act = action;
        }
        public void TryInvoke()
        {
            if (CheckGuard())
                return;
            act.Invoke();
        }
        bool CheckGuard()
        {
            var ret = Guard?.Invoke() ?? true;
            if(!ret)
                MyDebug.Log("GuardedAction failed");
            return ret;
        }
        public GuardedAction AddGuard(Func<bool> fGuard)
        {
            Guard += fGuard;
            return this;
        }
    }

    public class GuardedAction<T1>
    {
        readonly Action<T1> act;
        public event Func<bool> Guard;
        public GuardedAction(Action<T1> action)
        {
            act = action;
        }
        public void TryInvoke(T1 t1)
        {
            if (CheckGuard())
                return;
            act.Invoke(t1);
        }
        bool CheckGuard()
        {
            var ret = Guard?.Invoke() ?? true;
            if(!ret)
                MyDebug.Log($"GuardedAction<{typeof(T1).Name}> failed");
            return ret;
        }
        public GuardedAction<T1> AddGuard(Func<bool> fGuard)
        {
            Guard += fGuard;
            return this;
        }
    }

    public class GuardedAction<T1, T2>
    {
        readonly Action<T1, T2> act;
        public event Func<bool> Guard;
        public GuardedAction(Action<T1, T2> action)
        {
            act = action;
        }
        public void TryInvoke(T1 t1, T2 t2)
        {
            if (CheckGuard())
                return;
            act.Invoke(t1, t2);
        }
        bool CheckGuard()
        {
            var ret = Guard?.Invoke() ?? true;
            if(!ret)
                MyDebug.Log($"GuardedAction<{typeof(T1).Name}, {typeof(T2).Name}> failed");
            return ret;
        }
        public GuardedAction<T1, T2> AddGuard(Func<bool> fGuard)
        {
            Guard += fGuard;
            return this;
        }
    }

    public class GuardedFunc<TResult>
    {
        readonly Func<TResult> func;
        public event Func<bool> Guard;
        public GuardedFunc(Func<TResult> func)
        {
            this.func = func;
        }
        [CanBeNull]
        public TResult TryInvoke()
        {
            return !CheckGuard() ? default : func.Invoke();
        }

        bool CheckGuard()
        {
            var ret = Guard?.Invoke() ?? true;
            if(!ret)
                MyDebug.Log($"GuardedFunc<{typeof(TResult).Name}> failed");
            return ret;
        }
    }

    public class GuardedFunc<T1, TResult>
    {
        readonly Func<T1, TResult> func;
        public event Func<bool> Guard;
        public GuardedFunc(Func<T1, TResult> func)
        {
            this.func = func;
        }
        [CanBeNull]
        public TResult TryInvoke(T1 t1)
        {
            return !CheckGuard() ? default : func.Invoke(t1);
        }

        bool CheckGuard()
        {
            var ret = Guard?.Invoke() ?? true;
            if (!ret)
                MyDebug.Log($"GuardedFunc<{typeof(T1).Name}, {typeof(TResult).Name}> failed");
            return ret;
        }
    }
}