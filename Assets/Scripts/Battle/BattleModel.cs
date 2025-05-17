using System;
using UnityEngine;
using System.Collections.Generic;

public enum EBattleState
{
    InRoom,
}
[Serializable]
class BattleData
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
            BattleState = nameof(EBattleState.InRoom),
        };
    }
}


public static class BattleModel 
{
    
    
    static BattleData battleData;
    static MyFSM<EBattleState> battleFSM;
    public static void Init()
    {
        battleFSM = new MyFSM<EBattleState>();
        battleData = Saver.Load<BattleData>("Data", typeof(BattleData).ToString());
        if (battleData?.PlayerData == null)
        {
            MyDebug.Log("EnterBattle NULL", LogType.State);
            battleData = BattleData.GetDefault();
            Save("Init BattleData");
        }
    }

    static void Save(string info = "")
    {
        MyDebug.Log("Save " +info, LogType.State);
        Saver.Save("Data", typeof(BattleData).ToString(), battleData);
    }
    public static void EnterNextRoomBattle()
    {
        //TODO 不做了
        // ChangeState(EBattleState.InRoom);
    }


    public static void Launch()
    {
        battleFSM.ChangeState(battleData.BattleState);
    }
    public static void ChangeState(EBattleState eState)
    {
        battleData.BattleState = eState.ToString();
        battleFSM.ChangeState(eState.ToString());
        Save($"Battle Change State :{eState.ToString()}");
    }

    #region Getter

    public static PlayerData PlayerData => battleData.PlayerData;
    public static MapData MapData => battleData.MapData;
    public static MyState GetState(EBattleState e) => battleFSM.GetState(e.ToString());

    #endregion
}


