using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public abstract class ModelBase<T> : MonoBehaviour where T : DataBase
{
    [SerializeField] [ReadOnly] protected T data = null!;

    public void ReadData(T fData)
    {
        data = fData;
        ReadDataInternal();
    }
    protected abstract void ReadDataInternal();
}

[Serializable]
public abstract class DataBase
{
    
}