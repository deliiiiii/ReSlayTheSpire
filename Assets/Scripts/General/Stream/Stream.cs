using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee;

public abstract class Maybe<T>
{
    sealed class Just(T value) : Maybe<T>
    {
        public T JustValue { get; } = value;
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
            Just just => just.JustValue,
            _ => default!,
        };
    public static implicit operator Maybe<T>(T value) => Of(value);
    public T Value => this switch
    {
        Just just => just.JustValue,
        _ => default!
    };
}

public interface IStream
{
    Task CallTriggerAsync();
}
public class Stream<T, TOut>(Func<T> startFunc, Func<T, Task<TOut>> triggerFuncAsync): 
    Dele<T>, IStream
{
    // public Stream(Func<T> startFunc, Func<T, TOut> triggerFunc) 
    //     : this(startFunc, x => Task.FromResult(triggerFunc(x))){ }

    internal readonly Func<T> startFunc = startFunc;
    internal readonly List<(Func<T, Task<Maybe<T>>>, string)> mappers = [];
    internal Func<T, Task<TOut>> triggerFuncAsync = triggerFuncAsync;
    internal Action<T>? onBegin;
    internal Func<T, Task>? onBeginAsync;
    internal Action<TOut>? onEnd;
    internal Func<TOut, Task>? onEndAsync;
    internal List<IStream> endStreams = [];
    
    public async Task CallTriggerAsync()
    {
        try
        {
            Maybe<T> sta = startFunc();
            if (!sta.HasValue)
            {
                Debug.LogWarning($"{startFunc.Method.Name} At Start " +
                                 $"has returned null.");
                return;
            }
            foreach (var mapper in mappers)
            {
                sta = await mapper.Item1(sta);
                if (sta.HasValue)
                    continue;
                Debug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                                 $"has returned null.");
                return;
            }
            onBegin?.Invoke(sta);
            await (onBeginAsync != null ? onBeginAsync(sta) : Task.CompletedTask);
            result = await triggerFuncAsync(sta);
            onEnd?.Invoke(result);
            await (onEndAsync != null ? onEndAsync(result) : Task.CompletedTask);
            foreach (var endStream in endStreams)
            {
                _ = endStream.CallTriggerAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }
    internal Maybe<TOut> result { get; set; } = Maybe<TOut>.Nothing.Instance;
    
}
