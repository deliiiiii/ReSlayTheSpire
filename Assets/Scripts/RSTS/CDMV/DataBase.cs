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

public abstract class DataBaseSingle<TData>;

[Serializable]
public abstract class DataBaseMulti<TData, TAttribute, TConfigMulti>
    where TData : DataBaseMulti<TData, TAttribute, TConfigMulti>
    where TAttribute : IDAttribute
    where TConfigMulti : ConfigMulti<TConfigMulti>, new()
{
    static Dictionary<int, Func<TData>> ctorDic = [];
    public static void InitCtorDic()
    {
        ctorDic.Clear();
        var subTypeDic = typeof(TData).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(TData))
                        && x.GetAttribute<TAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<TAttribute>().ID);
        foreach (var config in RefPoolMulti<TConfigMulti>.Acquire())
        {
            if (!subTypeDic.TryGetValue(config.ID, out var type))
            {
                MyDebug.LogError($"InitCtorDic: {typeof(TData).Name} ID{config.ID} not found" +
                                 $", ID will be 0 as default");
                // ctorDic.Add(config.ID, () => new Card_Template
                // {
                //     ID = config.ID,
                //     Config = config,
                // });
                ctorDic.Add(config.ID, ctorDic[0]);
                continue;
            }
            ctorDic.Add(config.ID, () =>
            {
                var ins = (Activator.CreateInstance(type) as TData)!;
                ins.ID = config.ID;
                ins.Config = config;
                return ins;
            });
        }
    }
    
    public static TData CreateData(int id)
    {
        if (ctorDic.TryGetValue(id, out var func))
        {
            return func();
        }
        throw new Exception($"{typeof(TData).Name} CreateData : ID {id} out of range");
    }
    
    
    [JsonIgnore]
    public TConfigMulti Config = null!;

    public required int ID
    {
        get;
        set
        {
            field = value;
            Config = RefPoolMulti<TConfigMulti>.Acquire().FirstOrDefault(c => c.ID == value)!;
        }
    }
}

/// <summary>
/// TAttribute为IDAttribute
/// </summary>
public abstract class DataBaseMulti<TData, TConfigMulti> : DataBaseMulti<TData, IDAttribute, TConfigMulti>
    where TData : DataBaseMulti<TData, IDAttribute, TConfigMulti>
    where TConfigMulti : ConfigMulti<TConfigMulti>, new();