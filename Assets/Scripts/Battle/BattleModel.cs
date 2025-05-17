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


public class BattleModel : ModelBase
{
    BattleData battleData;
    MyFSM<EBattleState> battleFSM;
    public override void Init()
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
    public override void Launch()
    {
        battleFSM.ChangeState(battleData.BattleState);
    }

    void Save(string info = "")
    {
        MyDebug.Log("Save " +info, LogType.State);
        Saver.Save("Data", typeof(BattleData).ToString(), battleData);
    }
    public void EnterNextRoomBattle()
    {
        //TODO 不做了
        // ChangeState(EBattleState.InRoom);
    }


    
    public void ChangeState(EBattleState eState)
    {
        battleData.BattleState = eState.ToString();
        battleFSM.ChangeState(eState.ToString());
        Save($"Battle Change State :{eState.ToString()}");
    }

    #region Getter

    public PlayerData PlayerData => battleData.PlayerData;
    public MapData MapData => battleData.MapData;
    public MyState GetState(EBattleState e) => battleFSM.GetState(e.ToString());

    #endregion
}


