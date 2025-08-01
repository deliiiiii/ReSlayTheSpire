using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[Serializable]
public abstract class SceneItemData : DataBase
{
    public readonly SceneItemConfig Config;
    public HashSet<EBoxDir> OccupyDirSet = [];
    public abstract string GetDes();

    public Color DesColor() => this switch
    {
        PurpleSceneItemData => Color.magenta,
        _ => Color.black,
    };
    
    public SceneItemData(SceneItemConfig config)
    {
        Config = config;
    }
}

public class PurpleSceneItemData : SceneItemData
{
    public int Count;
    public int Energy => (Config as PurpleSceneItemConfig)!.Energy;

    public PurpleSceneItemData(PurpleSceneItemConfig config, int count) : base(config)
    {
        Count = count;
    }

    public override string GetDes()
    {
        return $"休息一下: +{Energy} 精力";
    }
}