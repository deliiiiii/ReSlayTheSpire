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
public class Stream<T>(Func<T>? startFunc = null, Func<T, Task>? triggerFuncAsync = null): 
    Dele<T>, IStream
{
    public Stream(Func<T>? startFunc) : this(startFunc, null){}
    public Stream(Func<T>? startFunc = null, Action<T>? triggerFunc = null) 
        : this(startFunc, x => { triggerFunc?.Invoke(x); return Task.CompletedTask; }){ }

    internal readonly Func<T> startFunc = startFunc ?? (() => default!);
    internal readonly List<(Func<T, Task<Maybe<T>>>, string)> mappers = [];
    internal Func<T, Task>? triggerFuncAsync = triggerFuncAsync;
    internal Action<T>? onBegin;
    internal Func<T, Task>? onBeginAsync;
    internal Action<T>? onEnd;
    internal Func<T, Task>? onEndAsync;
    internal IStream? endStream;
    
    public async Task CallTriggerAsync()
    {
        try
        {
            result = startFunc();
            if (!result.HasValue)
            {
                Debug.LogWarning($"{startFunc.Method.Name} At Start " +
                                 $"has returned null.");
                return;
            }
            foreach (var mapper in mappers)
            {
                result = await mapper.Item1(result);
                if (result.HasValue)
                    continue;
                Debug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Item1.Method.Name} " +
                                 $"has returned null.");
                return;
            }
            onBegin?.Invoke(result);
            await (onBeginAsync != null ? onBeginAsync(result) : Task.CompletedTask);
            await (triggerFuncAsync != null ? triggerFuncAsync(result) : Task.CompletedTask);
            onEnd?.Invoke(result);
            await (onEndAsync != null ? onEndAsync(result) : Task.CompletedTask);
            await (endStream != null ? endStream.CallTriggerAsync() : Task.CompletedTask);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }
    
    public T StartValue => startFunc();
    internal Maybe<T> result { get; set; } = Maybe<T>.Nothing.Instance;
    
}
