using System;
using System.Collections.Generic;

namespace Violee;

[Serializable]
public class SceneItemData : DataBase
{
    public readonly SceneItemConfig Config;
    public HashSet<EBoxDir> OccupyDirSet = [];
    
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
}