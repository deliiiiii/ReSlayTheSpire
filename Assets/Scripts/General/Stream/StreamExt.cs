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
    
    
    // public static Func<Unit> ToFunc(Action action)
    //     => () => { action(); return default; };
    // public static Func<T1, Unit> ToFunc<T1>(Action<T1> action)
    //     => (t1) => { action(t1); return default; };
    // public static Func<T1, T2, Unit> ToFunc<T1, T2>(Action<T1, T2> action)
    //     => (t1, t2) => { action(t1, t2); return default; };

    public static Func<Unit> CreateBind()
        => Bind(Unit.Create);
    public static Func<T> Bind<T>(Func<T> bind)
        => bind;
    
    public static Stream<T, Unit> SetTrigger<T>(this Func<T> self, Action<T> act)
    {
        var trigger = new Func<T, Task<Unit>>(x =>
        {
            act(x);
            return Task.FromResult(Unit.Create());
        });
        return SetTriggerAsync(self, trigger);
    }
    public static Stream<T, TOut> SetTrigger<T, TOut>(this Func<T> self, Func<T, TOut> act)
    {
        var trigger = new Func<T, Task<TOut>>(x => Task.FromResult(act(x)));
        return SetTriggerAsync(self, trigger);
    }
    
    public static Stream<T, TOut> SetTriggerAsync<T, TOut>(this Func<T> self, Func<T, Task<TOut>> func)
    {
        return new Stream<T, TOut>(startFunc: self, triggerFuncAsync: func);
    }
}