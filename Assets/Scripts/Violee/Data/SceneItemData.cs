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
    public SceneItemData DeepCopy()
    {
        if (this is PurpleSceneItemData p)
        {
            return new PurpleSceneItemData
            {
                OccupyDirSet = [..OccupyDirSet],
                StaminaCost = StaminaCost,
                DesPre = DesPre,
                HasCount = HasCount,
                Count = Count,
                OnRunOut = OnRunOut,
                Energy = p.Energy
            };
        }
        return new SceneItemData
        {
            OccupyDirSet = [..OccupyDirSet],
            StaminaCost = StaminaCost,
            DesPre = DesPre,
            HasCount = HasCount,
            Count = Count,
            OnRunOut = OnRunOut
        };
    }
    public GameObject Obj = null!;
    public HashSet<EBoxDir> OccupyDirSet = [];
    public int StaminaCost;
    public string DesPre = string.Empty;
    public bool HasCount;
    [ShowIf(nameof(HasCount))]
    public int Count;
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
            Count--;
            if (Count <= 0)
            {
                OnRunOut?.Invoke();
            }
        }
        UseEffect();
    }
    public bool CanUse()
    {
        if (HasCount)
            return Count > 0;
        return true;
    }
    public Color DesColor() => this switch
    {
        {StaminaCost : > 0} => Color.blue,
        _ => Color.white,
    };
    
    protected virtual void UseEffect(){}

    public SceneItemData ReadSth(SceneItemC2D param)
    {
        OccupyDirSet = param.DirSet;
        return this;
    }
}


[Serializable]
public class PurpleSceneItemData : SceneItemData
{
    [Header("Purple")]
    public int Energy;
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