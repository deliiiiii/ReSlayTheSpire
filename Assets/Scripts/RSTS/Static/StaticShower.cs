using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RSTS;

public class StaticShower : Singleton<StaticShower>
{
    [ShowInInspector] string GameState => GameFSM.CurStateNameStatic;
    [ShowInInspector] static string BattleState => BattleFSM.CurStateNameStatic;
    [ShowInInspector] static string YieldCardState => BothTurnFSM.CurStateNameStatic;
}