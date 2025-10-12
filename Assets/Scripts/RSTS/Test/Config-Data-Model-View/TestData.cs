using System;
using System.Collections.Generic;
using System.Linq;

namespace RSTS.Test;


public abstract class DataBase
{
    [NonSerialized]
    readonly HashSet<ConfigBase> configs = [];
    public ConfigBase AddConfig(ConfigBase config)
    {
        configs.Add(config);
        return config;
    }

    protected T GetConfig<T>() where T : ConfigBase
    {
        return (configs.First(c => c is T) as T)!;
    }

    protected DataBase(params ConfigBase[] configArr)
    {
        foreach (var config in configArr)
        {
            configs.Add(config);
        }
    }
}

public abstract class DataBase<T>(T config) : DataBase(config)
    where T : ConfigBase
{
    public T Config => GetConfig<T>();
}

[Serializable]
public class TestData(TestConfig config) : DataBase<TestConfig>(config)
{
    public List<int> SpreadIdList = [];
}