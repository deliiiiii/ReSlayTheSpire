using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

namespace RSTS.Test;


public abstract class DataBase
{
    [NonSerialized]
    readonly HashSet<ConfigBase> configSet = [];

    [PublicAPI]
    public static T Create<T>(params ConfigBase[] configArr) where T : DataBase, new()
    {
        var ret = new T();
        ret.configSet.AddRange(configArr);
        ret.OnReadConfig();
        return ret;
    }
    
    [Obsolete("At least ONE config is required.")][PublicAPI]
    public static T Create<T>()where T : DataBase, new() => Create<T>([]);
    protected abstract void OnReadConfig();
    protected T GetConfig<T>() where T : ConfigBase
    {
        var config = configSet.OfType<T>().FirstOrDefault();
        if (config == null)
        {
            throw new Exception($"Config of type {typeof(T).Name} not found");
        }
        return config;
    }
    protected DataBase(){}
}

public abstract class DataBase<T> : DataBase
    where T : ConfigBase
{
    public T Config => GetConfig<T>();
}

[Serializable]
public class TestData : DataBase<TestConfig>
{
    public List<int> SpreadIdList = [];

    protected override void OnReadConfig()
    {
        for (var i = 0; i < Config.SpreadCount; i++)
        {
            SpreadIdList.Add(new Random().Next(0, Config.SpreadMaxId));
        }
    }
}