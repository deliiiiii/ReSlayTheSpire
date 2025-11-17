using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RSTS;

public class StaticShower : Singleton<StaticShower>
{
    #region State
    const string NotInPlayMode = "Not in Play Mode";
    [ShowInInspector] static string GameState => GetState(GameStateWrap.One);
    [ShowInInspector] static string BattleState => GetState(BattleStateWrap.One);
    [ShowInInspector] static string YieldCardState => GetState(BothTurnStateWrap.One);
    static string GetState<TEnum, TArg>(StateWrap<TEnum, TArg> one)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
        => !Application.isPlaying ? NotInPlayMode : MyFSM.ShowState(one);
    #endregion
}