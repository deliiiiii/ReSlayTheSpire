using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

/// <summary>
/// C2D : Config to Data
/// </summary>
public struct SceneItemC2D(HashSet<EBoxDir> dirSet, Transform parent)
{
    public readonly HashSet<EBoxDir> DirSet = dirSet;
    public readonly Transform Parent = parent;
}

[Serializable]
public class SceneItemData : DataBase
{
    [NonSerialized] public readonly Transform Parent;
    [NonSerialized ]public readonly HashSet<EBoxDir> OccupyDirSet;
    [NonSerialized] public SceneItemModel ObjIns = null!;
    public SceneItemData(SceneItemConfig config, SceneItemC2D param)
    {
        Obj = config.Object;
        OccupyDirSet = param.DirSet;
        Parent = param.Parent;
        if (config.HasCount)
        {
            HasCount = true;
            count = config.Count;
        }
    }
    public GameObject Obj;
    public readonly bool HasCount;
    int count;
    public event Action? OnRunOut;
    
    public virtual string GetDes() => "Simple Item...";
    public void Use()
    {
        if (HasCount)
        {
            count--;
            if (count <= 0)
            {
                OnRunOut?.Invoke();
            }
        }
        UseEffect();
    }
    public bool CanUse()
    {
        if (HasCount)
            return count > 0;
        return true;
    }
    public Color DesColor() => this switch
    {
        PurpleSceneItemData => Color.magenta,
        _ => Color.black,
    };
    
    protected virtual void UseEffect(){}

    public static SceneItemData ReadConfig<T>(T config, SceneItemC2D param) where T : SceneItemConfig
    {
        if (config is PurpleSceneItemConfig purpleConfig)
        {
            return new PurpleSceneItemData(purpleConfig, param);
        }
        return new SceneItemData(config, param);
    }
}


[Serializable]
public class PurpleSceneItemData(PurpleSceneItemConfig config, SceneItemC2D param)
    : SceneItemData(config, param)
{
    public int Energy = config.Energy;
    public override string GetDes()
    {
        return $"休息一下: +{Energy} 精力";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        PlayerManager.AddEnergy(Energy);
    }
}