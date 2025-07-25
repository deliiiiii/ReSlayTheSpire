using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public abstract class ModelBase<TData> : MonoBehaviour, IModelBase<TData> where TData : DataBase
{
    [SerializeField] [ReadOnly] protected TData data = null!;
    public TData Data => data;

    public void ReadData(TData fData)
    {
        data = fData;
        OnReadData();
    }
    protected abstract void OnReadData();
    
}

public interface IModelBase<out TData> where TData : DataBase
{ 
    TData Data { get; }
}

public interface IModelManagerBase<out TModel> where TModel : IModelBase<DataBase>
{
    TModel ModelPrefab { get; }
}

[Serializable]
abstract class ModelManagerBase<TModel, TModelManager> : Singleton<TModelManager>, IModelManagerBase<TModel> 
    where TModel : MonoBehaviour, IModelBase<DataBase>
    where TModelManager : Singleton<TModelManager>
{
    static readonly MyKeyedCollection<Type, IModelManagerBase<IModelBase<DataBase>>> modelManagers
        = new(m => m.GetType());

    protected static T? GetModelManager<T>()
        where T : class, IModelManagerBase<IModelBase<DataBase>>
    {  
        if(modelManagers.Contains(typeof(T)))
            return modelManagers[typeof(T)] as T;
        return null;
    }

    protected static T? AddModelManager<T>(T modelManager)
        where T : class, IModelManagerBase<IModelBase<DataBase>>
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