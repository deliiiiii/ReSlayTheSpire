using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void Launch();

    internal static Dictionary<Type, ModelBase> modelDic = new();

    // protected static MainModel MainModel => modelDic[typeof(MainModel)] as MainModel;
    // protected static BattleModel BattleModel => modelDic[typeof(BattleModel)] as BattleModel;
}