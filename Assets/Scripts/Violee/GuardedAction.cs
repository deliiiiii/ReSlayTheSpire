using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Violee
{
    public class GuardedAction(Action action)
    {
        public event Func<bool>? Guard;

        public void TryInvoke()
        {
            if (CheckGuard())
                return;
            action.Invoke();
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

    public class GuardedAction<T1>(Action<T1> action)
    {
        public event Func<bool>? Guard;

        public void TryInvoke(T1 t1)
        {
            if (CheckGuard())
                return;
            action.Invoke(t1);
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

    public class GuardedAction<T1, T2>(Action<T1, T2> action)
    {
        public event Func<bool>? Guard;

        public void TryInvoke(T1 t1, T2 t2)
        {
            if (CheckGuard())
                return;
            action.Invoke(t1, t2);
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

    public class GuardedFunc<TResult>(Func<TResult> func)
    {
        public event Func<bool>? Guard;

        public TResult? TryInvoke()
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

    public class GuardedFunc<T1, TResult>(Func<T1, TResult> func)
    {
        public event Func<bool>? Guard;

        public TResult? TryInvoke(T1 t1)
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