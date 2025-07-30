using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public interface IModelBase;
public abstract class ModelBase<TData> : MonoBehaviour, IModelBase where TData : DataBase
{
    [ReadOnly] protected TData data = null!;
    public void ReadData(TData fData)
    {
        data = fData;
        OnReadData();
    }
    protected abstract void OnReadData();
}


[Serializable]
public abstract class ModelManagerBase<TModel, TModelManager> : Singleton<TModelManager>
    where TModel : MonoBehaviour, IModelBase
    where TModelManager : Singleton<TModelManager>
{
    [SerializeField] TModel modelPrefab = null!;
    public TModel ModelPrefab => modelPrefab;
    protected ObjectPool<TModel> modelPool = null!;

    protected override void Awake()
    {
        base.Awake();
        modelPool = new ObjectPool<TModel>(ModelPrefab, transform, 42);
    }
}


[Serializable]
public abstract class DataBase;