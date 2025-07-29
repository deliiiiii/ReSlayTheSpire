using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public interface IModelBase;
public abstract class ModelBase<TData> : MonoBehaviour, IModelBase where TData : DataBase
{
    [SerializeField] [ReadOnly] protected TData data = null!;
    public void ReadData(TData fData)
    {
        data = fData;
        OnReadData();
    }
    protected abstract void OnReadData();
}



[Serializable]
abstract class ModelManagerBase<TModel, TModelManager> : Singleton<TModelManager>
    where TModel : MonoBehaviour, IModelBase
    where TModelManager : Singleton<TModelManager>
{
    static readonly MyKeyedCollection<Type, Singleton<TModelManager>> modelManagers
        = new(m => m.GetType());

    protected static T? GetModelManager<T>()
        where T : Singleton<TModelManager>
    {  
        if(modelManagers.Contains(typeof(T)))
            return modelManagers[typeof(T)] as T;
        return null;
    }

    static T? AddModelManager<T>(T modelManager)
        where T : Singleton<TModelManager>
    {
        if(modelManagers.Contains(typeof(T)))
            return modelManagers[typeof(T)] as T;
        modelManagers.Add(modelManager);
        return modelManager;
    }
    
    
    
    [SerializeField] TModel modelPrefab = null!;
    public TModel ModelPrefab => modelPrefab;
    protected ObjectPool<TModel> modelPool = null!;

    protected override void Awake()
    {
        base.Awake();
        AddModelManager(this);
        modelPool = new ObjectPool<TModel>(ModelPrefab, transform, 42);
    }
}

[Serializable]
public abstract class DataBase
{
    
}