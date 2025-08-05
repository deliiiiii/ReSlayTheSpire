using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;


[Serializable]
public class SceneItemData : DataBase
{
    public SceneItemModel Model = null!;
    [ShowInInspector] public HashSet<EBoxDir> OccupyDirSet = [];
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

    public static SceneItemData CreateData(SceneItemData data, HashSet<EBoxDir> dirSet)
    {
        if (data is PurpleSceneItemData pData)
        {
            return new PurpleSceneItemData()
            {
                Model = data.Model,
                OccupyDirSet = [..dirSet],
                StaminaCost = data.StaminaCost,
                DesPre = data.DesPre,
                HasCount = data.HasCount,
                Count = data.Count,
                OnRunOut = data.OnRunOut,
                Energy = pData.Energy,
            };
        }
        return new SceneItemData()
        {
            Model = data.Model,
            OccupyDirSet = [..dirSet],
            StaminaCost = data.StaminaCost,
            DesPre = data.DesPre,
            HasCount = data.HasCount,
            Count = data.Count,
            OnRunOut = data.OnRunOut,
        };
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