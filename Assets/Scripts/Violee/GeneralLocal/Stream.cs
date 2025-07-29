using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Violee;

public abstract class Maybe<T>
{
    public sealed class Just(T value) : Maybe<T>
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

public class Stream<T>(Func<T> startFunc)
{
    readonly List<(Func<T, Task<Maybe<T>>>, string)> mappers = [];
    public event Action<T>? OnStart;
    public event Action<T>? OnEnd;
    
    public Stream<T> Map(Func<T, T> mapper, string logInfo = "")
    {   
        if (!mapper.Method.IsStatic)
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
        if (!predicate.Method.IsStatic)
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} .Where {predicate.Method.Name} must be static, " +
                               $"otherwise that added func is probably not PURE function !!");
            return this;
        }
        mappers.Add((value => Task.FromResult(predicate(value) ? Maybe<T>.Of(value) : Maybe<T>.Nothing.Instance), logInfo));
        return this;
    }

    public Stream<T> Delay(int miliSeconds)
    {
        mappers.Add((value => Task.Delay(miliSeconds).ContinueWith(_ => Maybe<T>.Of(value)), $"Delay {miliSeconds}ms"));
        return this;
    }
    
    public async Task TriggerAsync(Func<T, Task> triggerFuncAsync)
    {
        Maybe<T> startValue = startFunc();
        if (!startValue.HasValue)
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} At Start " +
                               $"has returned null.");
            return;
        }
        OnStart?.Invoke(startValue);
        foreach (var mapper in mappers)
        {
            startValue = await mapper.Item1(startValue);
            if (startValue.HasValue)
                continue;
            MyDebug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                               $"has returned null.");
            return;
        }
        await triggerFuncAsync(startValue);
        OnEnd?.Invoke(startValue);
    }
    
    public void Trigger(Action<T> triggerFunc)
    {
        Maybe<T> startValue = startFunc();
        if (!startValue.HasValue)
        {
            MyDebug.LogWarning($"{startFunc.Method.Name} At Start " +
                               $"has returned null.");
            return;
        }
        OnStart?.Invoke(startValue);
        foreach (var mapper in mappers)
        {
            startValue = Task.FromResult(mapper.Item1(startValue)).Result.Result;
            if (startValue.HasValue)
                continue;
            MyDebug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                               $"has returned null.");
            return;
        }
        triggerFunc(startValue);
        OnEnd?.Invoke(startValue);
    }
}