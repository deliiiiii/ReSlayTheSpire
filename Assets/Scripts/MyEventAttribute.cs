using System;
[AttributeUsage(AttributeTargets.Method)]
public class MyEventAttribute : Attribute
{
    public readonly Type EventType;
    
    public MyEventAttribute()
    {
        // 事件类型将从方法参数推断
    }
    public MyEventAttribute(Type eventType)
    {
        this.EventType = eventType;
    }
}