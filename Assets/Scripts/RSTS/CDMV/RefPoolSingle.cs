﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RSTS.CDMV;

public interface ISingleRef;
public interface IMultiRef;

public static class RefPoolSingle<T> where T : class, ISingleRef
{
    static readonly Dictionary<Type, T> oneDataDic = [];
    public static T Register(Func<T> ctor)
    {
        var type = typeof(T);
        if (!oneDataDic.ContainsKey(type))
            return oneDataDic[type] = ctor();
        MyDebug.LogError("DataBase: RegisterOne<" + typeof(T).Name + "> Duplicated");
        return oneDataDic[type];
    }
    public static T Acquire()
    {
        var type = typeof(T);
        if (oneDataDic.TryGetValue(type, out var data))
        {
            return data;
        }
        MyDebug.LogError("DataBase: AcquireOne<" + typeof(T).Name + "> Not Exist, Auto Register");
        return null!;
    }
    public static void Release()
    {
        var type = typeof(T);
        if (oneDataDic.Remove(type))
            return;
        MyDebug.LogError("DataBase: ReleaseOne<" + typeof(T).Name + "> Not Exist");
    }
}

public static class RefPoolMulti<T> where T : class, IMultiRef
{
    static readonly Dictionary<Type, List<T>> listDataDic = [];
    public static T RegisterOne(Func<T> ctor)
    {
        RegisterSome(ctor, 1, out var added, out _);
        return added.First();
    }
    public static void RegisterSome(Func<T> ctor, int duplicateCount)
        => RegisterSome(ctor, duplicateCount, out _, out _);
    public static void RegisterSome(Func<T> ctor, int duplicateCount, out List<T> addedList, out List<T> allInList)
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
        allInList = listDataDic[type].ToList();
    }
    public static List<T> Acquire()
    {
        var type = typeof(T);
        if (listDataDic.TryGetValue(type, out var data))
        {
            return data.ToList();
        }
        MyDebug.LogError("DataBase: AcquireList<" + typeof(T).Name + "> Not Exist");
        return [];
    }
    public static void ReleaseOne(T toRemove)
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
    public static void ReleaseAll()
    {
        var type = typeof(T);
        if (listDataDic.Remove(type))
            return;
        MyDebug.LogError("DataBase: ReleaseListAll<" + typeof(T).Name + "> Not Exist");
    }
}