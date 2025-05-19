using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void Launch();

    public static readonly Dictionary<Type, ModelBase> ModelDic = new();
}