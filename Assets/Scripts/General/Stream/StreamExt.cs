using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Unit = System.ValueTuple;

namespace Violee;


public class Dele<T>;
public static class Streamer
{
    // public static bool GreaterFull<TClass, T>(TClass tClass, Func<TClass, T> func, T val)
    //     where T : IComparable<T>
    // {
    //     return func(tClass).CompareTo(val) > 0;
    // }
    // public static Func<T, bool> Greater<TClass, T>(this TClass tClass, Func<TClass, T> func)
    //     where T : IComparable<T>
    //     => val => GreaterFull(tClass, func, val);
    // public static (T1, T2, T3) AddTuple<T1, T2, T3>(this (T1, T2) t12, T3 t3)
    // {
    //     return (t12.Item1, t12.Item2, t3);
    // }
    // public static Func<T1> LateGet<T1>(this T1 t1, Func<T1, T1>? func = null)
    // {
    //     func ??= _ => t1;
    //     return () => func(t1);
    // }
    // 下面这个实际上是Select
    // public static Func<T2> Curry<T1, T2>(this T1 t1, Func<T1, T2> func) 
    //     => () => func(t1);
    // 下面这个Select实际上也是Bind...
    // public static Func<T2> Select<T1, T2>(this T1 t1, Func<T1, T2> func) 
    //     => () => func(t1);
    // Reduce别名...
    // public static Func<T3> Merge<T1, T2, T3>(this Func<T1> t1, Func<T2> t2, Func<(T1, T2), T3> func) 
    //     => () => func((t1(), t2()));
    
    // 太难用...不对，这不是SelectMany吗。这个还是，Bind！
    // public static Func<T2> FlatMap<T1, T2>(this Func<T1> t, Func<T1, Func<T2>> flatMap) 
    //     => () => flatMap(t())();
    
    
    // static void F()
    // {
    //     // 如何以C#的视角理解Scala的fold，iterate，scan
    //     // List(1,2,3,4).foldLeft(0)(_+_)           返回10
    //     // Stream.iterate(1)(_+1).take(4).toList    返回 List(1,2,3,4)
    //     // List(1,2,3,4).scanLeft(0)(_+_)            返回List(0,1,3,6,10)
    //     
    //     // 1 foldLeft
    //     var list = new List<int> {1, 2, 3, 4};
    //     int result = list.Aggregate(0, (acc, x) => acc + x);  
    //     
    //     
    //     // 2 iterate
    //     // 方法1: 使用Range
    //     var result1 = Enumerable.Range(1, 4).ToList();  // [1,2,3,4]
    //     // 方法2: 自定义无限序列
    //     IEnumerable<int> InfiniteSequence(int start)
    //     {
    //         while (true) yield return start++;
    //     }
    //     var result2 = InfiniteSequence(1).Take(4).ToList();  // [1,2,3,4]
    //     
    //     
    //     // 3 scanLeft
    //     static IEnumerable<TAccumulate> ScanLeft<TSource, TAccumulate>(
    //         this IEnumerable<TSource> source, 
    //         TAccumulate seed, 
    //         Func<TAccumulate, TSource, TAccumulate> func)
    //     {
    //         TAccumulate accumulator = seed;
    //         yield return accumulator;
    //
    //         foreach (var item in source)
    //         {
    //             accumulator = func(accumulator, item);
    //             yield return accumulator;
    //         }
    //     }
    //
    //     // 使用
    //     var scanned = new List<int> {1, 2, 3, 4}.ScanLeft(0, (acc, x) => acc + x).ToList();
    //     // 返回 [0, 1, 3, 6, 10]
    // }

    // public static Func<T1> Bind<T1>(this T1 t1, Func<T1>? bind = null)
    // {
    //     bind ??= () => t1;
    //     return () => bind();
    // }
    
    // // Match可能还得改改
    // public static T2 Match<T1, T2>(this T1 t, Func<T1, T2> successFunc, Func<T2> failFunc) 
    //     => t is T2 ? successFunc(t) : failFunc();
    
    
    // public static IEnumerable<T2> Bind<T1, T2>(IEnumerable<T1> t1, Func<T1, IEnumerable<T2>> bind)
    // {
    //     return bind(t1.First());// 从First拿到Next...
    // }
    
    
    public static Func<Unit> ToFunc(Action action)
        => () => { action(); return default; };
    public static Func<T1, Unit> ToFunc<T1>(Action<T1> action)
        => (t1) => { action(t1); return default; };
    public static Func<T1, T2, Unit> ToFunc<T1, T2>(Action<T1, T2> action)
        => (t1, t2) => { action(t1, t2); return default; };
    
    
    public static Func<T> Bind<T>(Func<T> bind) 
        => bind;
    public static Func<T2> Bind<T1, T2>(this T1 t1, Func<T2> bind) 
        => bind;
    
    public static Func<T2> Map<T1, T2>(this Func<T1> t, Func<T1, T2> map) 
        => () => map(t());
    public static T Reduce<T>(this Func<T> t)
        => t();
    public static T2 Reduce<T1, T2>(this Func<T1> t1, T2 seed, Func<(T1, T2), T2> reduce) 
        => reduce((t1(), seed));
    public static Func<T> Reduce<T>(this Func<T> t, T seed, Func<T, T, T> reduce) 
        => () => reduce(t(), seed);
    
    
    
    // public static Dele<T2> Bind<T1, T2>(this Dele<T1> t1, Func<T1, Dele<T2>> bind)
    //     => bind(t1.Value);
    // public static Dele<T2> Map<T1, T2>(this Dele<T1> t1, Func<T1, T2> map)
    //     => map(t1.Value).WrapToDele();
    //
    // public static Dele<T> Reduce<T>(this Dele<T> t1, T seed, Func<T, T, T> reduce) 
    //     => reduce(t1.Value, seed).WrapToDele();
    
    public static Func<(T1, T2)> WithA<T1, T2>(this Func<T1> t1, Func<T2> t2)
        => () => (t1(), t2());
    
    public static Func<T1> DeleteA<T1, T2>(this Func<(T1, T2)> t12) 
        => () => t12().Item1;
    
    public static Action<T2> Curry<T1, T2>(this T1 t1, Action<T1, T2> action) 
        => t2 => action(t1, t2);


    public static Stream<Unit> ToStream<T>(this T t, Action action)
        => new(startFunc: () => new Unit(), triggerFunc: _ => action());
    public static Stream<T> ToStream<T>(this Func<T> t, Action<T> action) 
        => new(startFunc: t, triggerFunc:action);
    public static Stream<T> ToStreamAsync<T>(this Func<T> t, Func<T, Task> actionAsync) 
        => new(startFunc: t, triggerFuncAsync: actionAsync);
    
    
    static bool CheckValidMethod(MethodInfo methodInfo) => methodInfo.IsStatic || methodInfo.Name.Contains("b__");
    public static Stream<T> Map<T>(this Stream<T> self, Func<T, T> mapper, string logInfo = "")
    {   
        // method可以是lambda表达式, lambda表达式函数名包含b__
        if (!CheckValidMethod(mapper.Method))
        {
            Debug.LogWarning($"{self.startFunc.Method.Name} .Map {mapper.Method.Name} must be static, " +
                             $"otherwise that added func is probably not PURE function !!");
            return self;
        }
        self.mappers.Add((x => Task.FromResult(Maybe<T>.Of(mapper(x))), logInfo));
        return self;
    }

    public static Stream<T> Where<T>(this Stream<T> self, Predicate<T> predicate, string logInfo = "")
    {
        if (!CheckValidMethod(predicate.Method))
        {
            Debug.LogWarning($"{self.startFunc.Method.Name} .Where {predicate.Method.Name} must be static, " +
                               $"otherwise that added func is probably not PURE function !!");
            return self;
        }
        self.mappers.Add((value => Task.FromResult(predicate(value) ? Maybe<T>.Of(value) : Maybe<T>.Nothing.Instance), logInfo));
        return self;
    }

    public static Stream<T> Delay<T>(this Stream<T> self, int millSeconds)
    {
        self.mappers.Add((value => Task.Delay(millSeconds).ContinueWith(_ => Maybe<T>.Of(value)), $"Delay {millSeconds}ms"));
        return self;
    }

    public static Stream<T> SetTrigger<T>(this Stream<T> self, Action<T> triggerFunc)
    {
        return SetTriggerAsync(self, x =>
        {
            triggerFunc(x);
            return Task.CompletedTask;
        });
    }
    public static Stream<T> SetTriggerAsync<T>(this Stream<T> self, Func<T, Task> fTriggerFuncAsync)
    {
        self.triggerFuncAsync = fTriggerFuncAsync;
        return self;
    }

    public static Stream<T> EndWith<T>(this Stream<T> self, IStream fEndStream)
    {
        self.endStream = fEndStream;
        return self;
    }
    public static Stream<T> OnBegin<T>(this Stream<T> self, Action<T> action)
    {
        self.onBegin += action;
        return self;
    }
    public static Stream<T> OnBeginAsync<T>(this Stream<T> self, Func<T, Task> func)
    {
        self.onBeginAsync += func;
        return self;
    }
    public static Stream<T> OnEnd<T>(this Stream<T> self, Action<T> action)
    {
        self.onEnd += action;
        return self;
    }
    
    
    public static Stream<T> OnEndAsync<T>(this Stream<T> self, Func<T, Task> func)
    {
        self.onEndAsync += func;
        return self;
    }
    public static Stream<T> RemoveOnEndAsync<T>(this Stream<T> self, Func<T, Task> func)
    {
        self.onEndAsync -= func;
        return self;
    }
}

class TestData
{
    public string Name = string.Empty;
    public int Age;
    public bool IsGender;
}
