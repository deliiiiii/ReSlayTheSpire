using System;
using UnityEngine;
using System.Collections.Generic;

#region Card
public enum CardColor
{
    None,
    Red,
}
public enum CardType
{
    Attack,
    Skill,
    Ability,
    Curse,
}
[Serializable]
public class CardData
{
    private CardData(){}
    public int CardId;
    public bool IsUpper;
    public string CardName => "testCardName";
    public int CardCost => IsUpper ? 2 : 1;
    public CardColor CardColor => CardColor.Red;
    public CardType CardType => CardType.Attack;
    public CardData(int cardId, bool isUpper)
    {
        CardId = cardId;
        IsUpper = isUpper;
    }
    public CardData GetUpperCard()
    {
        return new CardData(CardId, true);
    }
}
#endregion
#region Bottle
[Serializable]
public class BottleData
{
    public int BottleId;
    public int BottleCount;
}
#endregion
#region Player
[Serializable]
public class PlayerData
{
    public int MaxHP;
    public int CurHP;
    public int Coin;
    public List<CardData> DeckCards;
    public List<BottleData> Bottles;
}
#endregion
#region Map
public enum MapNodeType
{
    NormalEnemy,
    EliteEnemy,
    Event,
    Rest,
    Shop,
    Boss,
}

[Serializable]
public class MapNodeData
{
    public int NodeID;
    public MapNodeType MapNodeType;
    public Vector2 Pos;
    public List<int> NextNodes;
}
[Serializable]
public class MapData
{
    public List<MapNodeData> MapNodes;
}
#endregion
[Serializable]
public class BattleData
{
    public bool HasPreBattleBonus;
    public string BattleSeed;
    public PlayerData PlayerData;
    public MapData MapData;
    public string BattleState;
}

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
            battleData = new BattleData
            {
                BattleSeed = "112233seed",
                PlayerData = new PlayerData
                {
                    MaxHP = 75,
                    CurHP = 75,
                    Coin = 99,
                    DeckCards = new List<CardData>
                    {
                        new(1,false),
                        new(1,true),
                        new(1,true),
                        new(2,false),
                        new(2,false),
                        new(2,true),
                        new(2,true),
                        new(3,false),
                        new(4,false),
                    },
                    Bottles = new List<BottleData>()
                },
                MapData = new MapData
                {
                    MapNodes = new List<MapNodeData>
                    {
                        new()
                        {
                            NodeID = 0,
                            MapNodeType = MapNodeType.NormalEnemy,
                            Pos = new Vector2(0, 0),
                            NextNodes = new List<int> {1,2,3},
                        },
                    },
                },
            };
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


