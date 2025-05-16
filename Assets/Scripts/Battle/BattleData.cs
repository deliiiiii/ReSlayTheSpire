using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleData
{
    public bool HasPreBattleBonus;
    public string BattleSeed;
    public PlayerData PlayerData;
    public MapData MapData;
    public string BattleState;
    
    public static BattleData GetDefault()
    {
        return new BattleData
        {
            BattleSeed = "112233seed",
            PlayerData = PlayerData.GetDefault(),
            MapData = MapData.GetDefault(),
        };
    }
}