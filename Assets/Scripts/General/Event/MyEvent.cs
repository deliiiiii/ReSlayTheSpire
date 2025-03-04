using System;
using System.Collections.Generic;
using System.Reflection;

public static class MyEvent
{
    private static Dictionary<Type, Delegate> eventHandlers = new Dictionary<Type, Delegate>();

    public static void ClearAll()
    {
        eventHandlers = new();
    }
    // 添加事件监听
    public static void AddListener<T>(Action<T> handler)
    {
        var type = typeof(T);
        
        if (!eventHandlers.ContainsKey(type))
        {
            eventHandlers[type] = handler;
        }
        else
        {
            eventHandlers[type] = Delegate.Combine(eventHandlers[type], handler);
        }
    }

    // 移除事件监听
    public static void RemoveListener<T>(Action<T> handler)
    {
        var type = typeof(T);
        
        if (eventHandlers.ContainsKey(type))
        {
            eventHandlers[type] = Delegate.Remove(eventHandlers[type], handler);
            
            if (eventHandlers[type] == null)
            {
                eventHandlers.Remove(type);
            }
        }
    }

    // 触发事件
    public static void Fire<T>(T eventData)
    {
        var type = typeof(T);
        
        if (eventHandlers.TryGetValue(type, out var handler))
        {
            (handler as Action<T>)?.Invoke(eventData);
        }
    }

    public static void RegisterAnnotatedHandlers(object instance)
    {
        Type instanceType = instance.GetType();
        // 查找所有带有MyEvent特性的方法
        foreach (var method in instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attributes = method.GetCustomAttributes(typeof(MyEventAttribute), true);
            foreach (MyEventAttribute attr in attributes)
            {
                Type eventType;
                // 首先检查是否在特性中指定了事件类型（向后兼容）
                if (attr.EventType != null)
                {
                    eventType = attr.EventType;
                }
                else
                {
                    // 从方法的第一个参数获取事件类型
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        MyDebug.LogError($"Method {method.Name} in {instanceType.Name} has MyEvent attribute but no parameters.");
                        continue;
                    }
                    eventType = parameters[0].ParameterType;
                }

                Type delegateType = typeof(Action<>).MakeGenericType(eventType);
                Delegate handler = Delegate.CreateDelegate(delegateType, instance, method);
                // 调用泛型AddListener方法
                typeof(MyEvent)
                    .GetMethod(nameof(AddListener))
                    .MakeGenericMethod(eventType)
                    .Invoke(null, new object[] { handler });
            }
        }
    }
    
}
