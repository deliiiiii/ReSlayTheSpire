using System;
using UnityEngine;
using System.Collections.Generic;
public partial class MainData
{
    public BattleData BattleData;
}

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
    public CardData(int f_cardId, bool f_isUpper)
    {
        CardId = f_cardId;
        IsUpper = f_isUpper;
    }
    public CardData GetUpperCard()
    {
        return new CardData(CardId, true);
    }
}
#endregion
[Serializable]
public class PlayerData
{
    public string Job;
    public int MaxHP;
    public int CurHP;
    public int Coin;
    public List<CardData> deckCards;
    public override string ToString()
    {
        return $"Job: {Job}, MaxHP: {MaxHP}, CurHP: {CurHP}, Coin: {Coin}, deckCards: {deckCards.ToString()}";
    }
}

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
    public List<int> nextNodes;
}
[Serializable]
public class MapData
{
    public List<MapNodeData> mapNodes;
}

[Serializable]
public class BattleData
{
    public string battleSeed;
    public PlayerData PlayerData;
    public MapData MapData;
    public override string ToString()
    {
        return $"battleSeed: {battleSeed}, PlayerData: {PlayerData.ToString()}, MapData: {MapData.ToString()}";
    }
}

public class BattleModel
{
    static BattleData battleData;
    public static void EnterBattle()
    {
        battleData = Saver.Load<BattleData>("Data", typeof(BattleData).ToString());
        if (battleData == null || battleData.PlayerData == null)
        {
            MyDebug.Log("EnterBattle NULL", LogType.State);
            battleData = new BattleData()
            {
                battleSeed = "112233seed",
                PlayerData = new PlayerData()
                {
                    Job = MainModel.SelectJobModel.GetSelectedJob().ToString(),
                    MaxHP = 75,
                    CurHP = 75,
                    Coin = 99,
                    deckCards = new()
                    {
                        new(1,true),
                        new(2,true),
                        new(3,false),
                    }
                },
                MapData = new MapData()
                {
                    mapNodes = new()
                    {
                        new MapNodeData()
                        {
                            NodeID = 0,
                            MapNodeType = MapNodeType.NormalEnemy,
                            Pos = new Vector2(0, 0),
                            nextNodes = new(){1,2,3},
                        },
                    },
                },
            };
            Save("Init BattleData");
        }
        // MyDebug.Log(mainData.PlayerName, LogType.State);
        // MyDebug.Log(battleData.PlayerData.Job, LogType.State);
        // MyDebug.Log(battleData.PlayerData.MaxHP, LogType.State);
        // MyDebug.Log(battleData.PlayerData.CurHP, LogType.State);
        // MyDebug.Log(battleData.PlayerData.Coin, LogType.State);
        MyEvent.Fire(new OnEnterBattleEvent()
        {
            PlayerName = MainModel.PlayerName,
            Job = battleData.PlayerData.Job,
            MaxHP = battleData.PlayerData.MaxHP,
            CurHP = battleData.PlayerData.CurHP,
            Coin = battleData.PlayerData.Coin,
        });
    }

    public static void Save(string info = "")
    {
        MyDebug.Log("Save " +info, LogType.State);
        Saver.Save("Data", typeof(BattleData).ToString(), battleData);
    }
}



public class OnEnterBattleEvent
{
    public string PlayerName;
    public string Job;
    public int MaxHP;
    public int CurHP;
    public int Coin;
}


