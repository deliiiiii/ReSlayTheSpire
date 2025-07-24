using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract partial class ViewBase : MonoBehaviour
{
    public virtual void IBL(){}
    static Dictionary<Type, ModelBase> modelDic => ModelBase.ModelDic;
    protected static ModelBase RegisterModel(ModelBase model)
    {
        if (modelDic.ContainsKey(model.GetType()))
        {
            return modelDic[model.GetType()];
        }
        modelDic.Add(model.GetType(), model);
        return model;
    }
    [CanBeNull]static T GetModel<T>() where T : ModelBase => modelDic[typeof(T)] ? modelDic[typeof(T)] as T: null;
}