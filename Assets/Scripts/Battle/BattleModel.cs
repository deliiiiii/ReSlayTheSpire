using System;
using UnityEngine;
using System.Collections.Generic;

public static class BattleModel 
{
    static BattleData battleData;
    static MyFSM battleFSM = new();
    public static void EnterBattle()
    {
        battleFSM = new MyFSM();
        battleData = Saver.Load<BattleData>("Data", typeof(BattleData).ToString());
        if (battleData?.PlayerData == null)
        {
            MyDebug.Log("EnterBattle NULL", LogType.State);
            battleData = BattleData.GetDefault();
            Save("Init BattleData");
        }
        battleFSM.ChangeState(Type.GetType(battleData.BattleState));
    }

    static void Save(string info = "")
    {
        MyDebug.Log("Save " +info, LogType.State);
        Saver.Save("Data", typeof(BattleData).ToString(), battleData);
    }
    public static void EnterNextRoomBattle()
    {
        battleData.BattleState = typeof(InRoomBattleState).ToString();
        battleFSM.ChangeState(Type.GetType(battleData.BattleState));
        Save("EnterNextRoomBattle");
    }

    #region Getter

    public static PlayerData PlayerData => battleData.PlayerData;
    public static MapData MapData => battleData.MapData;

    #endregion
}



public struct OnEnterBattleArg
{
    public string PlayerName;
    public EJobType PlayerJob;
    public PlayerData PlayerData;
    public MapData MapData;
}


