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

public partial class MainModel
{
    public class BattleModel
    {

        public static void EnterBattle()
        {
            if (mainData.BattleData == null || mainData.BattleData.PlayerData == null)
            {
                MyDebug.Log("EnterBattle NULL", LogType.State);
                mainData.BattleData = new BattleData()
                {
                    battleSeed = "112233seed",
                    PlayerData = new PlayerData()
                    {
                        Job = mainData.SelectJobData.SelectedJob.ToString(),
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
                Save();
            }
            // MyDebug.Log(mainData.PlayerName, LogType.State);
            // MyDebug.Log(battleData.PlayerData.Job, LogType.State);
            // MyDebug.Log(battleData.PlayerData.MaxHP, LogType.State);
            // MyDebug.Log(battleData.PlayerData.CurHP, LogType.State);
            // MyDebug.Log(battleData.PlayerData.Coin, LogType.State);
            MyEvent.Fire(new OnEnterBattleEvent()
            {
                PlayerName = mainData.PlayerName,
                Job = mainData.BattleData.PlayerData.Job,
                MaxHP = mainData.BattleData.PlayerData.MaxHP,
                CurHP = mainData.BattleData.PlayerData.CurHP,
                Coin = mainData.BattleData.PlayerData.Coin,
            });
        }
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


