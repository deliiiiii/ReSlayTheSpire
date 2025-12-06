using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace RSTS;

public abstract class BattleState(BattleFSM fsm) : BattleFSM.StateBase(fsm);
public class BattleSelectLastBuff(BattleFSM fsm) : BattleState(fsm);
public class BattleLose(BattleFSM fsm) : BattleState(fsm);
public class BattleWin(BattleFSM fsm) : BattleState(fsm);

// public class BattleBothTurn : StateFSM<GameBattle, BattleState, BattleBothTurn, BothTurnState>
// {
//     // public BothTurnFSM BothTurnFSM = new BothTurnFSM(new BothTurnGrossStart());
//     
//     public HPAndBuffData PlayerHPAndBuffData = new();
//     public Observable<int> CurEnergy = new(5);
//     public Observable<int> MaxEnergy = new(5);
//     public Observable<int> PlayerCurHP => PlayerHPAndBuffData.CurHP;
//     public Observable<int> PlayerMaxHP => PlayerHPAndBuffData.MaxHP;
//     public Observable<int> PlayerBlock => PlayerHPAndBuffData.Block;
//     public int TurnID;
//     [JsonIgnore]public MyList<EnemyDataBase> EnemyList = [];
//     public MyList<CardInTurn> HandList = [];
//     public MyList<CardInTurn> DrawList = [];
//     public MyList<CardInTurn> DiscardList = [];
//     public MyList<CardInTurn> ExhaustList = [];
//     int loseHpCount;
//     
//     /// 第一个参数，弃牌堆；第二个参数，点击后的回调
//     public event Action<List<CardInTurn>, Action<CardInTurn>>? OnOpenDiscardOnceClick;
//     public event Action<List<CardInTurn>, int, Action<CardInTurn>>? OnOpenHandOnceClick;
//     public event Action? OnPlayerLoseHP;
//
//     public override void Init()
//     {
//         PlayerHPAndBuffData.CurHP = Context.CurHP;
//         PlayerHPAndBuffData.MaxHP = Context.MaxHP;
//         PlayerHPAndBuffData.Block = new Observable<int>(0);
//         PlayerHPAndBuffData.CurHP.OnValueChangedFull += (oldV, newV) =>
//         {
//             if (newV < oldV)
//             {
//                 loseHpCount++;
//                 OnPlayerLoseHP?.Invoke();
//             }
//         };
//         TurnID = 0;
//         
//         Context.DeckList.ForEach(cardData =>
//         {
//             DrawList.MyAdd(CardInTurn.CreateByAttr(cardData.Config.ID, cardData));
//         });
//         DrawList.Shuffle();
//         
//         EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
//         EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
//         EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
//         
//         CollectAllCards().ForEach(cardInTurn => cardInTurn.OnEnterBothTurn());
//     }
//
//     public override void Bind()
//     {
//         
//     }
//
//     public override void UnInit()
//     {
//         
//     }
//     
//     IEnumerable<CardInTurn> CollectAllCards()
//     {
//         foreach (var card in HandList)
//             yield return card;
//         foreach (var card in DrawList)
//             yield return card;
//         foreach (var card in DiscardList)
//             yield return card;
//         foreach (var card in ExhaustList)
//             yield return card;
//     }
// }