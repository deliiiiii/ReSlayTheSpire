using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


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
    static T GetModel<T>() where T : ModelBase => modelDic[typeof(T)] ? modelDic[typeof(T)] as T: null;
}


#if UNITY_EDITOR
public partial class ViewBase
{
    protected static MainView MainView;
    protected static BattleView BattleView;
    protected static MainModel MainModel => GetModel<MainModel>();
    protected static BattleModel BattleModel => GetModel<BattleModel>();
    
    void Awake()
    {
        EditorApplication.playModeStateChanged -= OnExitPlayMode;
        EditorApplication.playModeStateChanged += OnExitPlayMode;
    }

    void OnExitPlayMode(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingPlayMode)
        {
            MainView = null;
            BattleView = null;
        }
    }
}
#endif