using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RSTS;

public class StaticShower : Singleton<StaticShower>
{
    [ShowInInspector] string GameState => FSM.Game.CurStateName;
    [ShowInInspector] static string BattleState => FSM.Game.Battle.CurStateName;
    [ShowInInspector] static string YieldCardState => FSM.Game.Battle.BothTurn.YieldCard.CurStateName;
}