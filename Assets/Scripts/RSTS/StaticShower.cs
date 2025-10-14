using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RSTS;

public class StaticShower : Singleton<StaticShower>
{
    #region State
    const string NotInPlayMode = "Not in Play Mode";
    [ShowInInspector]
    static string GameState => GetState<EGameState>();
    static string GetState<T>() where T : Enum 
        => !Application.isPlaying ? NotInPlayMode : MyFSM.ShowState<T>();
    #endregion
    
    #region Config
    [ShowInInspector]
    public List<CardConfigMulti> CardConfigs => RefPoolMulti<CardConfigMulti>.Acquire();
    // [ShowInInspector]
    // public List<BottleConfig> ItemConfigs => RefPool<BottleConfig>.AcquireList();
    #endregion

    [ShowInInspector]
    public SlotDataMulti? SlotData => RefPoolMulti<SlotDataMulti>.Acquire().FirstOrDefault();
}