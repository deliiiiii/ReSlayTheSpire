using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

namespace RSTS.CDMV;

public abstract class DataBase
{
    readonly HashSet<ConfigBase> configSet = [];

    [PublicAPI]
    internal static T Create<T>(params ConfigBase[] configParams) where T : DataBase, new()
    {
        var ret = new T();
        ret.configSet.AddRange(configParams);
        ret.OnReadConfig();
        return ret;
    }
    
    [Obsolete("At least ONE config is required.")][PublicAPI]
    internal static T Create<T>()where T : DataBase, new() => Create<T>([]);
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
    protected T Config => GetConfig<T>();
}