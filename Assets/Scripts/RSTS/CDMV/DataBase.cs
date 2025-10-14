using System;
using System.Collections.Generic;
using System.Linq;

namespace RSTS.CDMV;

public abstract class DataBase
{
    static readonly Dictionary<Type, DataBase> oneDataDic = [];
    static readonly Dictionary<Type, List<DataBase>> listDataDic = [];
    protected DataBase()
    {
        OnReadConfig();
    }
    protected virtual void OnReadConfig(){}
}
