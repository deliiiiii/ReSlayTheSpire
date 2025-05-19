using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract class ViewBase : MonoBehaviour
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

    protected static MainView MainView;
    protected static BattleView BattleView;
    
    protected static MainModel MainModel => modelDic[typeof(MainModel)] as MainModel;
    protected static BattleModel BattleModel => modelDic[typeof(BattleModel)] as BattleModel;
}