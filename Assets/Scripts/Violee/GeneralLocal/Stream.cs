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
public class Stream<T>(Func<T>? startFunc = null): IStream
{
    public Func<T> StartFunc { get; set; } = startFunc ?? (() => default!);
    readonly List<(Func<T, Task<Maybe<T>>>, string)> mappers = [];
    Func<T, Task>? triggerFuncAsync;
    public event Action<T>? OnBegin;
    public event Func<T, Task>? OnBeginAsync;
    public event Action<T>? OnEnd;
    IStream? endStream;
    Maybe<T> result;

    bool CheckValidMethod(MethodInfo methodInfo)
    {
        return methodInfo.IsStatic || methodInfo.Name.Contains("b__");
    }
    public Stream<T> Map(Func<T, T> mapper, string logInfo = "")
    {   
        // method可以是lambda表达式, lambda表达式函数名包含b__
        if (!CheckValidMethod(mapper.Method))
        {
            MyDebug.LogWarning($"{StartFunc.Method.Name} .Map {mapper.Method.Name} must be static, " +
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
            MyDebug.LogWarning($"{StartFunc.Method.Name} .Where {predicate.Method.Name} must be static, " +
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

    public IStream EndWith<T2>(Stream<(T, T2)> fEndStream, T2 thatAddedStartValue)
    {
        endStream = fEndStream;
        (endStream as Stream<(T, T2)>)!.StartFunc = () => (result, thatAddedStartValue);
        return this;
    }

    public async Task CallTriggerAsync()
    {
        result = StartFunc();
        if (!result.HasValue)
        {
            MyDebug.LogWarning($"{StartFunc.Method.Name} At Start " +
                               $"has returned null.");
            return;
        }
        foreach (var mapper in mappers)
        {
            result = await mapper.Item1(result);
            if (result.HasValue)
                continue;
            MyDebug.LogWarning($"{StartFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                               $"has returned null.");
            return;
        }
        OnBegin?.Invoke(result);
        await (OnBeginAsync != null ? OnBeginAsync(result) : Task.CompletedTask);
        await (triggerFuncAsync != null ? triggerFuncAsync(result) : Task.CompletedTask);
        OnEnd?.Invoke(result);
        await (endStream != null ? endStream.CallTriggerAsync() : Task.CompletedTask);
    }
}