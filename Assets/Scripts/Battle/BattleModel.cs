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
#region Kusuri
[Serializable]
public class KusuriData
{
    public int KusuriId;
    public int KusuriCount;
}
#endregion
#region Player
[Serializable]
public class PlayerData
{
    public string Job;
    public int MaxHP;
    public int CurHP;
    public int Coin;
    public List<CardData> DeckCards;
    public List<KusuriData> Kusuris;
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
    public List<int> nextNodes;
}
[Serializable]
public class MapData
{
    public List<MapNodeData> mapNodes;
}
#endregion
[Serializable]
public class BattleData
{
    public bool hasPreBattleBonus;
    public string battleSeed;
    public PlayerData PlayerData;
    public MapData MapData;

    public string BattleState;

    public string CurSelectedEnemyType;
}

public class BattleModel
{
    static BattleData battleData;
    static MyFSM battleFSM;
    public static void EnterGlobalBattle()
    {
        battleFSM = new();
        battleData = Saver.Load<BattleData>("Data", typeof(BattleData).ToString());
        if (battleData == null || battleData.PlayerData == null)
        {
            MyDebug.Log("EnterBattle NULL", LogType.State);
            battleData = new BattleData()
            {
                battleSeed = "112233seed",
                PlayerData = new PlayerData()
                {
                    Job = MainModel.SelectJobModel.SelectedJob.ToString(),
                    MaxHP = 75,
                    CurHP = 75,
                    Coin = 99,
                    DeckCards = new()
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
                    Kusuris = new()
                    {
                    },
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
                BattleState = typeof(SelectNextRoomState).ToString(),
                CurSelectedEnemyType = "Enemy1",
            };
            Save("Init BattleData");
        }
        MyEvent.Fire(new OnEnterBattleEvent()
        {
            PlayerName = MainModel.PlayerName,
            PlayerData = battleData.PlayerData,
            MapData = battleData.MapData,
        });
        battleFSM.ChangeState(Type.GetType(battleData.BattleState));
    }

    public static void Save(string info = "")
    {
        MyDebug.Log("Save " +info, LogType.State);
        Saver.Save("Data", typeof(BattleData).ToString(), battleData);
    }
    
    public static string CurSelectedEnemyType => battleData.CurSelectedEnemyType;
    public static void SetCurSelectedEnemyType(string enemyType)
    {
        battleData.CurSelectedEnemyType = enemyType;
        Save("SetCurSelectedEnemyType");
    }
}



public class OnEnterBattleEvent
{
    public string PlayerName;
    public PlayerData PlayerData;
    public MapData MapData;
}


