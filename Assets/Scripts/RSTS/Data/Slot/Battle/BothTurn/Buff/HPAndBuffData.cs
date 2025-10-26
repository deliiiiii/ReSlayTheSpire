using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace RSTS;
[Serializable]
public class HPAndBuffData
{
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Block = new(0);
    [SerializeReference] List<BuffDataBase> buffList = [];
    public event Action<BuffDataBase>? OnAddBuff;
    public event Action<BuffDataBase>? OnRemoveBuff;
    public event Action<BuffDataBase, int>? OnChangeBuffStack;
    // public bool HasBuff<T>(out T buffData) where T : BuffDataBase
    // {
    //     buffData = buffList.OfType<T>().First();
    //     return buffData == null;
    // }
    bool HasBuff(Type type, out BuffDataBase buffData)
        => (buffData = buffList.FirstOrDefault(buff => buff.GetType() == type)!) != null;

    public void AddBuff(BuffDataBase addedBuff)
    {
        if(HasBuff(addedBuff.GetType(), out var existBuff))
        {
            if (existBuff is { StackInfo: not null } && addedBuff.StackInfo != null)
            {
                existBuff.StackInfo.ChangeCount(addedBuff.StackInfo.Count);
            }
            else
            {
                MyDebug.Log("Add Buff " + addedBuff.GetType().Name + " Cannot Stack");
            }
            return;
        }
        buffList.Add(addedBuff);
        if (addedBuff.StackInfo != null)
        {
            addedBuff.StackInfo.Count.OnValueChangedAfter += v 
                => OnChangeBuffStack?.Invoke(addedBuff, v);
        }
        OnAddBuff?.Invoke(addedBuff);
    }

    public void RemoveBuff(BuffDataBase removedBuff)
    {
        buffList.Remove(removedBuff);
        OnRemoveBuff?.Invoke(removedBuff);
    }
    public void ClearBuff()
    {
        while (true)
        {
            var count = buffList.Count;
            if (count == 0)
                break;
            var toRemove = buffList[count - 1];
            buffList.RemoveAt(count - 1);
            OnRemoveBuff?.Invoke(toRemove);
        }
    }

    public int GetAtkBaseAddSum(Predicate<IBuffAtkBaseAdd>? conditionModify = null, Func<IBuffAtkBaseAdd, int>? modifier = null)
    {
        conditionModify ??= _ => false;
        modifier ??= buff => buff.GetAtkBaseAdd();
        return buffList
            .OfType<IBuffAtkBaseAdd>()
            .Sum(buff => conditionModify(buff) ? modifier(buff) : buff.GetAtkBaseAdd());
    }

    public float GetAtkFinalMulti()
    {
        return buffList
            .OfType<IBuffAtkFinalMul>()
            .Aggregate(1f, (seed, buff) => seed * (1 + buff.GetAtkFinalMulti()));
    }

    public void UseABuff(EBuffUseTime eUseTime)
    {
        
    }
    public void DisposeABuff(EBuffDisposeTime eDisposeTime)
    {
        var toDispose = buffList
            .Where(buff => buff.DisposeTime == eDisposeTime);
        var toRemove = new List<BuffDataBase>();
        toDispose.ForEach(buffData =>
        {
            if(buffData.Dispose())
                toRemove.Add(buffData);
        });
        toRemove.ForEach(RemoveBuff);
    }
}