using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

/// <summary>
/// C2D : Config to Data
/// </summary>
public struct SceneItemC2D(HashSet<EBoxDir> dirSet)
{
    public readonly HashSet<EBoxDir> DirSet = dirSet;
}

[Serializable]
public class SceneItemData : DataBase
{
    [NonSerialized]public readonly HashSet<EBoxDir> OccupyDirSet;
    public SceneItemData(SceneItemConfig config, SceneItemC2D param)
    {
        OccupyDirSet = param.DirSet;
        Obj = config.Object;
        StaminaCost = config.StaminaCost;
        DesPre = config.DesPre;
        HasCount = config.HasCount;
        count = config.Count;
    }
    public GameObject Obj;
    public int StaminaCost;
    public string DesPre;
    public readonly bool HasCount;
    int count;
    public event Action? OnRunOut;

    public virtual string GetInteractDes()
    {
        var sb = new StringBuilder();
        sb.Append(DesPre);
        if (StaminaCost > 0)
            sb.Append($"消耗{StaminaCost}点体力,\n");
        return sb.ToString();
    }
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
        {StaminaCost : > 0} => Color.blue,
        _ => Color.white,
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
    public override string GetInteractDes()
    {
        var sb = new StringBuilder(base.GetInteractDes());
        sb.Append($"恢复{Energy}点精力");
        return sb.ToString();
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        PlayerManager.StaminaCount.Value -= StaminaCost;
        PlayerManager.EnergyCount.Value += Energy;
    }
}