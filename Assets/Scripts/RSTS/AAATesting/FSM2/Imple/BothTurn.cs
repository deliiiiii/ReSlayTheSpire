using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace RSTS;

public partial class BothTurn
{
    public HPAndBuffData PlayerHPAndBuffData = new();
    public Observable<int> CurEnergy = new(5);
    public Observable<int> MaxEnergy = new(5);
    public Observable<int> PlayerCurHP => PlayerHPAndBuffData.CurHP;
    public Observable<int> PlayerMaxHP => PlayerHPAndBuffData.MaxHP;
    public Observable<int> PlayerBlock => PlayerHPAndBuffData.Block;
    public int TurnID;
    [JsonIgnore]public MyList<EnemyDataBase> EnemyList = [];
    public MyList<CardInTurn> HandList = [];
    public MyList<CardInTurn> DrawList = [];
    public MyList<CardInTurn> DiscardList = [];
    public MyList<CardInTurn> ExhaustList = [];
    int loseHpCount;
    

    /// 第一个参数，弃牌堆；第二个参数，点击后的回调
    public event Action<List<CardInTurn>, Action<CardInTurn>>? OnOpenDiscardOnceClick;
    public event Action<List<CardInTurn>, int, Action<CardInTurn>>? OnOpenHandOnceClick;
    public event Action? OnPlayerLoseHP;

    BothTurnFSM bothTurnFSM;

    public BothTurn()
    {
        PlayerHPAndBuffData.CurHP = Battle.CurHP;
        PlayerHPAndBuffData.MaxHP = Battle.MaxHP;
        PlayerHPAndBuffData.Block = new Observable<int>(0);
        PlayerHPAndBuffData.CurHP.OnValueChangedFull += (oldV, newV) =>
        {
            if (newV < oldV)
            {
                loseHpCount++;
                OnPlayerLoseHP?.Invoke();
            }
        };
        TurnID = 0;
        
        Battle.DeckList.ForEach(cardData =>
        {
            DrawList.MyAdd(CardInTurn.CreateByAttr(cardData.Config.ID, cardData));
        });
        DrawList.Shuffle();
        
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        CollectAllCards().ForEach(cardInTurn => cardInTurn.OnEnterBothTurn());

        bothTurnFSM = new BothTurnFSM { Outer = this };
    }
    
    IEnumerable<CardInTurn> CollectAllCards()
    {
        foreach (var card in HandList)
            yield return card;
        foreach (var card in DrawList)
            yield return card;
        foreach (var card in DiscardList)
            yield return card;
        foreach (var card in ExhaustList)
            yield return card;
    }
}

public class BothTurnFSM : FSM2<BothTurn, BothTurnFSM, BothTurnState>;
public abstract class BothTurnState : BothTurnFSM.StateBase;
public class BothTurnGrossStart : BothTurnState;
public class BothTurnPlayerTurnStart : BothTurnState;
public class BothTurnPlayerDraw : BothTurnState;
public class BothTurnPlayerYieldCard : BothTurnState
{
    public CardModel CardModel;
}
public class BothTurnPlayerTurnEnd : BothTurnState;
public class BothTurnEnemyTurnStart : BothTurnState;
public class BothTurnEnemyAction : BothTurnState;
public class BothTurnEnemyTurnEnd : BothTurnState;