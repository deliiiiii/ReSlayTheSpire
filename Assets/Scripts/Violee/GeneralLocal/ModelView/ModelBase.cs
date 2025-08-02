using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public interface IModelBase;
public abstract class ModelBase<TData> : MonoBehaviour, IModelBase where TData : DataBase
{
    [ReadOnly] 
    [ShowInInspector]
    public TData Data = null!;
    public void ReadData(TData data)
    {
        Data = data;
        OnReadData();
    }
    protected abstract void OnReadData();
}