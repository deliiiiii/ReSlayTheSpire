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

/// 通过反射创建的数据基类，子类需添加IDAttribute特性
public abstract class DataAttr<TData, TAttribute>
    where TData : DataAttr<TData, TAttribute>
    where TAttribute : IDAttribute
{
    static readonly Dictionary<int, Func<TData>> ctorDic = [];
    public static void InitCtorDic()
    {
        ctorDic.Clear();
        typeof(TData).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(TData))
                        && x.GetAttribute<TAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<TAttribute>().ID)
            .ForEach(pair =>
            {
                ctorDic.Add(pair.Key, () => (TData)Activator.CreateInstance(pair.Value));
            });
    }
    
    protected static TData CreateByAttr(int id)
    {
        if (ctorDic.TryGetValue(id, out var ctor))
        {
            return ctor();
        }
        // 找不到对应ID的类时，返回ID为-1的FallBack类.
        return ctorDic[-1]();
        // throw new Exception($"{typeof(TData).Name} CreateData : ID {id} out of range");
    }
}
