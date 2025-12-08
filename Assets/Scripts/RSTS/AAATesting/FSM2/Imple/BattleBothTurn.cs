using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace RSTS;

public partial class BattleBothTurn : FSM2<BattleBothTurn>
{
    public HPAndBuffData PlayerHPAndBuffData = new();
    public Observable<int> CurEnergy = new(5);
    public Observable<int> MaxEnergy = new(5);
    public Observable<int> PlayerCurHP => PlayerHPAndBuffData.CurHP;
    public Observable<int> PlayerMaxHP => PlayerHPAndBuffData.MaxHP;
    public Observable<int> PlayerBlock => PlayerHPAndBuffData.Block;
    public event Action? OnPlayerLoseHP;
    public MyList<EnemyDataBase> EnemyList = [];
    
    public int TurnID;
    int loseHpCount;
    
    public class Card
    {
        public GameBattle.Card InGameBattle;
        public int TempUpgradeLevel;

        public Card(GameBattle.Card inGameBattle)
        {
            InGameBattle = inGameBattle;
            TempUpgradeLevel = InGameBattle.UpgradeLevel;
        }
    }
    public MyList<Card> HandList = [];
    public MyList<Card> DrawList = [];
    public MyList<Card> DiscardList = [];
    public MyList<Card> ExhaustList = [];
    /// 第一个参数，弃牌堆；第二个参数，点击后的回调
#pragma warning disable CS0067 // 事件从未使用过
    public event Action<List<Card>, Action<Card>>? OnOpenDiscardOnceClick;
    public event Action<List<Card>, int, Action<Card>>? OnOpenHandOnceClick;
#pragma warning restore CS0067 // 事件从未使用过


    public void OnEnter()
    {
        PlayerHPAndBuffData.CurHP = BelongFSM.CurHP;
        PlayerHPAndBuffData.MaxHP = BelongFSM.MaxHP;
        PlayerHPAndBuffData.Block = new Observable<int>(0);
        PlayerHPAndBuffData.CurHP.OnValueChangedFull += (oldV, newV) =>
        {
            if (newV < oldV)
            {
                loseHpCount++;
                OnPlayerLoseHP?.Invoke();
            }
        };
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        TurnID = 0;
        foreach (var cardData in BelongFSM.DeckList)
        {
            DrawList.MyAdd(new Card(cardData));
        }
        DrawList.Shuffle();
    }

    IEnumerable<Card> CollectAllCards()
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

// public class BothTurnFSM : FSM2<BattleBothTurnFSM, BothTurnFSM>;
// public class BothTurnGrossStart : BothTurnFSM.IState;
// public class BothTurnPlayerTurnStart : BothTurnFSM.IState;
// public class BothTurnPlayerDraw : BothTurnFSM.IState;
// public class BothTurnPlayerYieldCard : BothTurnFSM.IState
// {
//     public CardModel? CardModel;
// }
// public class BothTurnPlayerTurnEnd : BothTurnFSM.IState;
// public class BothTurnEnemyTurnStart : BothTurnFSM.IState;
// public class BothTurnEnemyAction : BothTurnFSM.IState;
// public class BothTurnEnemyTurnEnd : BothTurnFSM.IState;