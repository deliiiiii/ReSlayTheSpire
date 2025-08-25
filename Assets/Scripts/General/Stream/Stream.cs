using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Unit = System.ValueTuple;

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

public class Stream(Func<Unit> startFunc, Func<Unit, Task<Unit>> triggerFuncAsync)
    : Stream<Unit, Unit>(startFunc, triggerFuncAsync);
public class Stream<T, TOut>(Func<T?> startFunc, Func<T, Task<TOut>> triggerFuncAsync): 
    Dele<T>, IStream
{
    // public Stream(Func<T> startFunc, Func<T, TOut> triggerFunc) 
    //     : this(startFunc, x => Task.FromResult(triggerFunc(x))){ }

    readonly List<(Func<T, Task<T?>>, string)> mappers = [];
    Action<T>? onBegin;
    Func<T, Task>? onBeginAsync;
    Action<TOut>? onEnd;
    Func<TOut, Task>? onEndAsync;
    List<IStream> endStreams = [];
    
    public async Task CallTriggerAsync()
    {
        try
        {
            var sta = startFunc();
            if (sta == null)
            {
                Debug.LogWarning($"{startFunc.Method.Name} At Start " +
                                 $"has returned null.");
                return;
            }
            foreach (var mapper in mappers)
            {
                sta = await mapper.Item1(sta);
                if (sta != null)
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
    // Maybe<TOut> result { get; set; } = Maybe<TOut>.Nothing.Instance;
    TOut? result;
    
    static bool CheckValidMethod(MethodInfo methodInfo) => methodInfo.IsStatic || methodInfo.Name.Contains("b__");
    public Stream<T, TOut> Map(Func<T, T?> mapper, string logInfo = "")
    {   
        // method可以是lambda表达式, lambda表达式函数名包含b__
        if (!CheckValidMethod(mapper.Method))
        {
            Debug.LogWarning($"{startFunc.Method.Name} .Map {mapper.Method.Name} must be static, " +
                             $"otherwise that added func is probably not PURE function !!");
            return this;
        }
        mappers.Add((x => Task.FromResult(mapper(x)), logInfo));
        return this;
    }

    public Stream<T, TOut> Where(Predicate<T> predicate, string logInfo = "")
    {
        if (!CheckValidMethod(predicate.Method))
        {
            Debug.LogWarning($"{startFunc.Method.Name} .Where {predicate.Method.Name} must be static, " +
                               $"otherwise that added func is probably not PURE function !!");
            return this;
        }
        mappers.Add((value => Task.FromResult(predicate(value) ? value : default(T?)), logInfo));
        return this;
    }

    // public static Stream<T> Delay<T>(this Stream<T> self, int millSeconds)
    // {
    //     self.mappers.Add((value => Task.Delay(millSeconds).ContinueWith(_ => Maybe<T>.Of(value)), $"Delay {millSeconds}ms"));
    //     return self;
    // }

    
    public Stream<T, TOut> OnBegin(Action<T> act)
    {
        onBegin += act;
        return this;
    }
    public Stream<T, TOut> OnBeginAsync(Func<T, Task> func)
    {
        onBeginAsync += func;
        return this;
    }
    public Stream<T, TOut> OnEnd(Action<TOut> act)
    {
        onEnd += act;
        return this;
    }
    public Stream<T, TOut> OnEndAsync(Func<TOut, Task> func)
    {
        onEndAsync += func;
        return this;
    }
    public Stream<T, TOut> RemoveOnEndAsync(Func<TOut, Task> func)
    {
        onEndAsync -= func;
        return this;
    }
    
    public TOut? SelectResult()
    {
        return result;
    }
    public TOut2 SelectResult<TOut2>(Func<TOut?, TOut2> selector)
    {
        return selector(result);
    }
    public Func<TOut?> BindResult()
    {
        return () => result;
    }
    public Func<TOut2> BindResult<TOut2>(Func<TOut?, TOut2> selector)
    {
        return () => selector(result);
    }
    
    public Stream<TOut, Unit> Continue(Action<TOut> act)
    {
        var trigger = new Func<TOut, Task<Unit>>(x =>
        {
            act(x);
            return Task.FromResult(Unit.Create());
        });
        return ContinueAsync(trigger);
    }
    public Stream<TOut, TOut2> ContinueAsync<TOut2>(Func<TOut, Task<TOut2>> func)
    {
        var ret = new Stream<TOut, TOut2>(startFunc: () => result, triggerFuncAsync: func);
        endStreams.Add(ret);
        return ret;
    }
    // public Stream<T2> ContinueAsync<T, T2>(Func<T, T2> selector, Func<T2, Task> endAsync)
    // {
    //     var ret = new Stream<T2>(startFunc: () => selector(result), triggerFuncAsync: endAsync);
    //     endStream = ret;
    //     return ret;
    // }
    
}