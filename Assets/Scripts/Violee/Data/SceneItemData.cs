using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

// [Serializable]
public class SceneItemData : DataBase
{
    
    public SceneItemData(SceneItemConfig config)
    {
        if (config.HasCount)
        {
            HasCount = true;
            count = config.Count;
        }
    }
    
    public HashSet<EBoxDir> OccupyDirSet = [];
    // protected readonly SceneItemConfig Config;
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
    }
    public bool CanUse()
    {
        if (HasCount)
            return count >= 0;
        return true;
    }
    public Color DesColor() => this switch
    {
        PurpleSceneItemData => Color.magenta,
        _ => Color.black,
    };
}


[Serializable]
public class PurpleSceneItemData : SceneItemData
{
    public PurpleSceneItemData(PurpleSceneItemConfig config) : base(config)
    {
        Energy = config.Energy;
    }

    public int Energy;
    public override string GetDes()
    {
        return $"休息一下: +{Energy} 精力";
    }
}