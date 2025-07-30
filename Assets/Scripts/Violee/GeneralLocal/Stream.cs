using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Violee;

public abstract class Maybe<T>
{
    sealed class Just(T value) : Maybe<T>
    {
        public T Value { get; } = value;
    }

    public sealed class Nothing : Maybe<T>
    {
        public static Nothing Instance { get; } = new Nothing();
    }

    public bool HasValue => this != Nothing.Instance; 
    
    public static Maybe<T> Of(T value) => 
        value != null ? new Just(value) : Nothing.Instance;
    
    public static implicit operator T(Maybe<T> maybe) =>
        maybe switch
        {
            Just just => just.Value,
            Nothing _ => Nothing.Instance,
            _ => default!
        };
    public static implicit operator Maybe<T>(T value) => Of(value);
}

public interface IStream
{
    Task CallTriggerAsync();
}
public class Stream<T>(Func<T>? startFunc = null, Func<T, Task>? triggerFuncAsync = null, IStream? endStream = null): IStream
{
    public Stream(Func<T>? startFunc = null, Action<T>? triggerFunc = null) 
        : this(startFunc, x => { triggerFunc?.Invoke(x); return Task.CompletedTask; }){ }
    
    readonly Func<T> startFunc = startFunc ?? (() => default!);
    readonly List<(Func<T, Task<Maybe<T>>>, string)> mappers = [];
    Func<T, Task>? triggerFuncAsync = triggerFuncAsync;
    public event Action<T>? OnBegin;
    public event Func<T, Task>? OnBeginAsync;
    public event Action<T>? OnEnd;
    IStream? endStream = endStream;

    static bool CheckValidMethod(MethodInfo methodInfo) => methodInfo.IsStatic || methodInfo.Name.Contains("b__");

    public Stream<T> Map(Func<T, T> mapper, string logInfo = "")
    {   
        // method可以是lambda表达式, lambda表达式函数名包含b__
        if (!CheckValidMethod(mapper.Method))
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Method.Name} must be static, " +
                             $"otherwise that added func is probably not PURE function !!");
            return this;
        }
        mappers.Add((x => Task.FromResult(Maybe<T>.Of(mapper(x))), logInfo));
        return this;
    }

    public Stream<T> Where(Predicate<T> predicate, string logInfo = "")
    {
        if (!CheckValidMethod(predicate.Method))
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} .Where {predicate.Method.Name} must be static, " +
                               $"otherwise that added func is probably not PURE function !!");
            return this;
        }
        mappers.Add((value => Task.FromResult(predicate(value) ? Maybe<T>.Of(value) : Maybe<T>.Nothing.Instance), logInfo));
        return this;
    }

    public Stream<T> Delay(int millSeconds)
    {
        mappers.Add((value => Task.Delay(millSeconds).ContinueWith(_ => Maybe<T>.Of(value)), $"Delay {millSeconds}ms"));
        return this;
    }

    public Stream<T> SetTrigger(Action<T> triggerFunc)
    {
        return SetTriggerAsync(x =>
        {
            triggerFunc(x);
            return Task.CompletedTask;
        });
    }
    public Stream<T> SetTriggerAsync(Func<T, Task> fTriggerFuncAsync)
    {
        triggerFuncAsync = fTriggerFuncAsync;
        return this;
    }

    public Stream<T> EndWith(IStream fEndStream)
    {
        endStream = fEndStream;
        return this;
    }

    public async Task CallTriggerAsync()
    {
        Result = startFunc();
        if (!Result.HasValue)
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} At Start " +
                               $"has returned null.");
            return;
        }
        foreach (var mapper in mappers)
        {
            Result = await mapper.Item1(Result);
            if (Result.HasValue)
                continue;
            MyDebug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                               $"has returned null.");
            return;
        }
        OnBegin?.Invoke(Result);
        await (OnBeginAsync != null ? OnBeginAsync(Result) : Task.CompletedTask);
        await (triggerFuncAsync != null ? triggerFuncAsync(Result) : Task.CompletedTask);
        OnEnd?.Invoke(Result);
        await (endStream != null ? endStream.CallTriggerAsync() : Task.CompletedTask);
    }

    public Maybe<T> Result { get; private set; } = Maybe<T>.Nothing.Instance;
}