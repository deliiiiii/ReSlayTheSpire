using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract partial class ViewBase : MonoBehaviour
{
    public virtual void IBL(){}
    static Dictionary<Type, ViewBase> viewDic = new();
    static Dictionary<Type, ModelBase> modelDic => ModelBase.modelDic;


    public static ViewBase RegisterView(ViewBase view)
    {
        if (modelDic.ContainsKey(view.GetType()))
        {
            // MyDebug.LogError($"model {model.GetType()} already exists");
            return viewDic[view.GetType()];
        }
        viewDic.Add(view.GetType(), view);
        return view;
    }
    public static ModelBase RegisterModel(ModelBase model)
    {
        if (modelDic.ContainsKey(model.GetType()))
        {
            // MyDebug.LogError($"model {model.GetType()} already exists");
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

public partial class ViewBase
{
    static int staticCount = 0;
#if UNITY_EDITOR
    void Start()
    {
        if (staticCount == 0)
        {
            staticCount++;
            EditorApplication.playModeStateChanged += (s) =>
            {
                if (s != PlayModeStateChange.ExitingPlayMode)
                    return;
                viewDic.Clear();
                modelDic.Clear();
            };
        }
    }
#endif
}


public abstract class ModelBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void Launch();

    internal static Dictionary<Type, ModelBase> modelDic = new();

    // protected static MainModel MainModel => modelDic[typeof(MainModel)] as MainModel;
    // protected static BattleModel BattleModel => modelDic[typeof(BattleModel)] as BattleModel;
}

