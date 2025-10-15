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
    [ShowInInspector] static string GameState => GetState<EGameState>();
    [ShowInInspector] static string BattleState => GetState<EBattleState>();
    [ShowInInspector] static string YieldCardState => GetState<EPlayerTurn>();
    static string GetState<T>() where T : Enum 
        => !Application.isPlaying ? NotInPlayMode : MyFSM.ShowState<T>();
    #endregion
}