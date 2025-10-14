using System;
using System.Collections.Generic;
using System.Linq;

namespace RSTS.CDMV;

public static class RefPool<TBase> where TBase : class
{
    static readonly Dictionary<Type, TBase> oneDataDic = [];
    static readonly Dictionary<Type, List<TBase>> listDataDic = [];
    
    public static T RegisterOne<T>(Func<T> ctor) where T : TBase
    {
        var type = typeof(T);
        if (!oneDataDic.ContainsKey(type))
            return (T)(oneDataDic[type] = ctor());
        MyDebug.LogError("DataBase: RegisterOne<" + typeof(T).Name + "> Duplicated");
        return (T)oneDataDic[type];
    }
    public static bool TryAcquireOne<T>(out T gotten) where T : class, TBase
    {
        gotten = null!;
        var type = typeof(T);
        if (oneDataDic.TryGetValue(type, out var data))
        {
            gotten = (T)data;
            return true;
        }
        MyDebug.LogError("DataBase: AcquireOne<" + typeof(T).Name + "> Not Exist, Auto Register");
        return false;
    }
    public static bool TryAcquireOne(out TBase gotten) => TryAcquireOne<TBase>(out gotten);
    public static void ReleaseOne<T>() where T : TBase
    {
        var type = typeof(T);
        if (oneDataDic.Remove(type))
            return;
        MyDebug.LogError("DataBase: ReleaseOne<" + typeof(T).Name + "> Not Exist");
    }
    public static void ReleaseOne() => ReleaseOne<TBase>();
    
    public static void RegisterList<T>(Func<T> ctor, int duplicateCount)
        where T : TBase
        => RegisterList(ctor, duplicateCount, out _, out _);
    public static void RegisterList<T>(Func<T> ctor, int duplicateCount, out List<T> addedList, out List<T> allInList)
        where T : TBase
    {
        var type = typeof(T);
        if (!listDataDic.ContainsKey(type))
        {
            listDataDic[type] = [];
        }
        addedList = [];
        for (var i = 0; i < duplicateCount; i++)
        {
            var added = ctor();
            listDataDic[type].Add(added);
            addedList.Add(added);
        }
        allInList = listDataDic[type].OfType<T>().ToList();
    }
    public static List<T> AcquireList<T>() where T : TBase
    {
        var type = typeof(T);
        if (listDataDic.TryGetValue(type, out var data))
        {
            return data.OfType<T>().ToList();
        }
        MyDebug.LogError("DataBase: AcquireList<" + typeof(T).Name + "> Not Exist");
        return [];
    }
    public static List<TBase> AcquireList() => AcquireList<TBase>();
    public static void ReleaseListOne<T>(T toRemove) where T : TBase
    {
        var type = typeof(T);
        if (listDataDic.TryGetValue(type, out var data)) 
        {
            if (data.Remove(toRemove))
                return;
            MyDebug.LogError("DataBase: ReleaseListOne<" + typeof(T).Name + "> Not Exist In List");
            return;
        }
        MyDebug.LogError("DataBase: ReleaseListOne<" + typeof(T).Name + "> Not Exist");
    }
    public static void ReleaseListAll<T>() where T : TBase
    {
        var type = typeof(T);
        if (listDataDic.Remove(type))
            return;
        MyDebug.LogError("DataBase: ReleaseListAll<" + typeof(T).Name + "> Not Exist");
    }
    public static void ReleaseListAll() => ReleaseListAll<TBase>();
}