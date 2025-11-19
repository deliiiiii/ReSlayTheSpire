using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace RSTS.CDMV;

[AttributeUsage(AttributeTargets.Class)]
public class IDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}

/// 通过Config创建的数据接口，Json忽略Config字段，通过ID自动关联
public abstract class DataConfig<TData, TConfigMulti>
    where TData : DataConfig<TData, TConfigMulti>
    where TConfigMulti : ConfigMulti<TConfigMulti>, new()
{
    [JsonIgnore] public TConfigMulti Config = null!;
    [JsonProperty]
    int ID
    {
        set
        {
            field = value;
            Config = RefPoolMulti<TConfigMulti>.Acquire().FirstOrDefault(c => c.ID == field)!;
        }
    }
    public static TData CreateByConfig(int id)
    {
        var ret = Activator.CreateInstance<TData>();
        ret.ID = id;
        return ret;
    }
}

/// 通过反射创建的数据基类，子类需添加IDAttribute特性
public abstract class DataAttr<TData, TAttribute, TContext>
    where TData : DataAttr<TData, TAttribute, TContext>
    where TAttribute : IDAttribute
{
    static Dictionary<int, Func<TContext, TData>> ctorDic = [];
    public static void InitCtorDic()
    {
        ctorDic.Clear();
        typeof(TData).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(TData))
                        && x.GetAttribute<TAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<TAttribute>().ID)
            .ForEach(pair =>
            {
                ctorDic.Add(pair.Key, context =>
                {
                    var ins = (Activator.CreateInstance(pair.Value) as TData)!;
                    ins.ReadContext(context);
                    return ins;
                });
                // ctorDic.TryAdd(pair.Key, ctorDic[0]);
            });
    }
    
    public static TData CreateByAttr(int id, TContext context)
    {
        if (ctorDic.TryGetValue(id, out var func))
        {
            return func(context);
        }
        throw new Exception($"{typeof(TData).Name} CreateData : ID {id} out of range");
    }
    
    protected abstract void ReadContext(TContext context);
}
