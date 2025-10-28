using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace RSTS;
[Serializable]
public class BothTurnData : IMyFSMArg
{
    [NonSerialized]BattleData battleData;
    
    [SerializeReference]public HPAndBuffData PlayerHPAndBuffData;
    public Observable<int> CurEnergy = new(5);
    public Observable<int> MaxEnergy = new(5);
    public Observable<int> PlayerCurHP => PlayerHPAndBuffData.CurHP;
    public Observable<int> PlayerMaxHP => PlayerHPAndBuffData.MaxHP;
    public Observable<int> PlayerBlock => PlayerHPAndBuffData.Block;
    public int TurnID;
    [SerializeReference] public MyList<EnemyDataBase> EnemyList = [];
    [SerializeReference] public MyList<CardDataBase> HandList = [];
    [SerializeReference] public MyList<CardDataBase> DrawList = [];
    [SerializeReference] public MyList<CardDataBase> DiscardList = [];
    [SerializeReference] public MyList<CardDataBase> ExhaustList = [];
    /// 第一个参数，弃牌堆；第二个参数，点击后的回调
    public event Action<List<CardDataBase>, Action<CardDataBase>>? OnOpenDiscardOnceClick;
    
    public BothTurnData(BattleData battleData, MyFSM<EBothTurn> fsm)
    {
        this.battleData = battleData;
        PlayerHPAndBuffData = new HPAndBuffData
        {
            CurHP = battleData.CurHP,
            MaxHP = battleData.MaxHP,
            Block = new Observable<int>(0)
        };
        TurnID = 0;
        
        fsm.GetState(EBothTurn.GrossStart).OnEnter += () =>
        {
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerTurnStart);
        };
        
        fsm.GetState(EBothTurn.PlayerTurnStart).OnEnter += () =>
        {
            TurnID++;
            PlayerHPAndBuffData.Block.Value = 0;
            LoseEnergy(CurEnergy.Value);
            GainEnergy(MaxEnergy);
            PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
            PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
            
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDraw);
        };
        
        fsm.GetState(EBothTurn.PlayerDraw).OnEnter += () =>
        {
            DrawSome(5);
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard);
        };
        
        fsm.GetState(EBothTurn.PlayerYieldCard).OnEnter += () =>
        {
            MyFSM.Register(YieldCardStateWrap.One, EYieldCardState.None, _ => new YieldCardData());
        };
        
        fsm.GetState(EBothTurn.PlayerYieldCard).OnExit += () =>
        {
            MyFSM.Release(YieldCardStateWrap.One);
        };
        
        fsm.GetState(EBothTurn.PlayerDiscard).OnEnter += () =>
        {
            DiscardAllHand();
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerTurnEnd);
        };
        
        fsm.GetState(EBothTurn.PlayerTurnEnd).OnEnter += () =>
        {
            PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
            PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.EnemyTurnStart);
        };
        
        fsm.GetState(EBothTurn.EnemyTurnStart).OnEnter += () =>
        {
            EnemyList.ForEach(enemyData =>
            {
                enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
                enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
            });
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.EnemyTurnEnd);
        };
        
        fsm.GetState(EBothTurn.EnemyTurnEnd).OnEnter += () =>
        {
            EnemyList.ForEach(enemyData =>
            {
                enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
                enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
            });
            MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerTurnStart);
        };
    }

    public void Launch()
    {
        EnemyList.MyClear();
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        HandList.MyClear();
        DrawList.MyClear();
        DrawList.MyAddRange(battleData.DeckList);
        DrawList.Shuffle();
        DiscardList.MyClear();
        ExhaustList.MyClear();
    }

    public void UnInit()
    {
        EnemyList.MyClear();
        
        CollectAllCards().ForEach(cardData => cardData.OnExitBothTurn());
        
        HandList.MyClear();
        DrawList.MyClear();
        DiscardList.MyClear();
        ExhaustList.MyClear();
        
        PlayerHPAndBuffData.ClearBuff();
    }

    IEnumerable<CardDataBase> CollectAllCards()
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
    public bool DrawSome(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (!DrawOne())
                return false;
        }
        return true;
    }
    
    public void LoseEnergy(int lose)
    {
        CurEnergy.Value -= lose;
    }
    public void UseEnergy(int use)
    {
        CurEnergy.Value -= use;
    }
    public void GainEnergy(int gain)
    {
        CurEnergy.Value += gain;
    }

    bool DrawOne()
    {
        if (DrawList.Count == 0)
            RefillDrawList();
        if (DrawList.Count == 0)
        {
            Debug.LogError("没有牌可以抽了");
            return false;
        }
        var ret = DrawList[^1];
        DrawList.MyRemoveAt(DrawList.Count - 1);
        HandList.MyAdd(ret);
        return true;
    }

    public bool TryYield(CardDataBase toYield, out string failReason)
    {
        failReason = string.Empty;
        if(toYield.ContainsKeyword(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        if (CurEnergy < toYield.CurCostInfo switch
            {
                CardCostNumber costNumber => costNumber.Cost,
                CardCostX => CurEnergy,
                CardCostNone or _ => int.MaxValue,
            })
        {
            failReason = "能量不足";
            return false;
        }
        if(!toYield.YieldCondition(this, out failReason))
        {
            return false;
        }
        return true;
    }

    public void DiscardAllHand()
    {
        HandList.ForEach(card =>
        {
            if (card.ContainsKeyword(ECardKeyword.Ethereal))
            {
                ExhaustList.MyAdd(card);
                return;
            }
            // else if(card.Config.Category != ECardCategory.Ability)
            DiscardList.MyAdd(card);
        });
        HandList.MyClear();
    }

    public async UniTask YieldAsync(CardDataBase toYield)
    {
        int costEnergy = toYield.CurCostInfo switch
        {
            CardCostNumber number => number.Cost,
            CardCostX => CurEnergy,
            CardCostNone or _ => 0,
        };
        UseEnergy(costEnergy);
        if (toYield.Config.Category == ECardCategory.Ability)
        {
            // 打出能力牌，不会消耗
            TemporaryRemove(toYield);
        }
        else if (toYield.ContainsKeyword(ECardKeyword.Exhaust))
        {
            // 消耗
            ExhaustOne(toYield);
        }
        else
        {
            // 正常打出
            HandList.MyRemove(toYield);
            DiscardList.MyAdd(toYield);
        }
        await toYield.YieldAsync(this, costEnergy);
        if(EnemyList.Count == 0)
        {
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Win);
        }
    }
    
    void ExhaustOne(CardDataBase toExhaust)
    {
        HandList.MyRemove(toExhaust);
        ExhaustList.MyAdd(toExhaust);
    }

    void TemporaryRemove(CardDataBase toRemove)
    {
        HandList.MyRemove(toRemove);
    }

    
    void RefillDrawList()
    {
        DrawList.MyAddRange(DiscardList);
        DiscardList.MyClear();
        DrawList.Shuffle();
    }
    
    #region yield effect
    // 使用攻击牌攻击的伤害
    public void AttackEnemy(EnemyDataBase? enemyData, int baseAtk,int strengthMulti = 1)
    {
        if (enemyData == null)
            return;
        Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out var resultList, strengthMulti);
        if(resultList.OfType<AttackResultDie>().Any())
            EnemyList.MyRemove(enemyData);
    }
    public async Task AttackEnemyMultiTimesAsync(EnemyDataBase? enemyData, int baseAtk, int times)
    {
        for (int t = 0; t < times; t++)
        {
            AttackEnemy(enemyData, baseAtk);
            await Task.Delay(300);
        }
    }
    
    public void AttackEnemyRandomly(int baseAtk)
    {
        if(EnemyList.Count == 0)
            return;
        var enemyData = EnemyList.RandomItem();
        AttackEnemy(enemyData, baseAtk);
    }
    public async UniTask AttackEnemyRandomlyMultiTimesAsync(int baseAtk, int times)
    {
        for (int t = 0; t < times; t++)
        {
            AttackEnemyRandomly(baseAtk);
            await UniTask.Delay(300);
        }
    }

    public void AttackAllEnemies(int baseAtk)
    {
        // var allResult = new List<AttackResult>();
        var toRemoveList = new List<EnemyDataBase>();
        EnemyList.ForEach(enemyData =>
        {
            Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out var resultList);
            // allResult.AddRange(resultList);
            if (resultList.OfType<AttackResultDie>().Any())
            {
                toRemoveList.Add(enemyData);
            }
        });
        toRemoveList.ForEach(toRemove => EnemyList.MyRemove(toRemove));
    }
    
    public async UniTask AttackAllEnemiesMultiTimesAsync(int baseAtk, int times)
    {
        for (int t = 0; t < times; t++)
        {
            AttackAllEnemies(baseAtk);
            await UniTask.Delay(300);
        }
    }

    public void AttackPlayer(EnemyDataBase enemyData, int baseAtk)
    {
        Attack(enemyData.HPAndBuffData, PlayerHPAndBuffData, baseAtk, out var resultList);
        if (resultList.OfType<AttackResultDie>().Any())
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Lose);
    }

    public void LoseHPToPlayer(int loseHP)
    {
        PlayerHPAndBuffData.CurHP.Value -= loseHP;
        if (PlayerHPAndBuffData.CurHP <= 0)
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Lose);
    }
    
    public int UIGetAttackEnemyValue(EnemyDataBase? enemyData, int baseAtk
        , int strengthMultiFromCard4)
    {
        return GetAttackValue(PlayerHPAndBuffData, enemyData?.HPAndBuffData ?? null, baseAtk, strengthMultiFromCard4);
    }
    
    int GetAttackValue(HPAndBuffData from, HPAndBuffData? to, int baseAtk
        , int strengthMultiFromCard4 = 1)
    {
        var baseBuffAdd = from.GetAtkBaseAddSum(buff =>
        {
            if(buff is BuffDataStrength)
                return buff.GetAtkBaseAdd() * strengthMultiFromCard4;
            return buff.GetAtkBaseAdd();
        });
        var finalMulFrom = from.GetFromAtkFinalMulti();
        var finalMulTo = to?.GetToAtkFinalMulti() ?? 1f;
        return Mathf.FloorToInt((baseAtk + baseBuffAdd) * finalMulFrom * finalMulTo);
    }

    void Attack(HPAndBuffData from, HPAndBuffData to, int baseAtk, out List<AttackResult> resultList
        , int strengthMultiFromCard4 = 1)
    {
        resultList = [];
        if (!to.CanBeSelected)
            return;
        // (atk + strength) * (1 - weak) * (1 + vulnerable) * (1 + backAttack)
        var finalAtk = GetAttackValue(from, to, baseAtk, strengthMultiFromCard4);
        to.CurHP.Value -= finalAtk;
        if (to.CurHP <= 0)
        {
            resultList.Add(new AttackResultDie(to.CurHP));
        }
    }
    
    public void AddBuffToPlayer(BuffDataBase buffData)
    {
        PlayerHPAndBuffData.AddBuff(buffData);
    }
    public void AddBuffToEnemy(EnemyDataBase? enemyData, BuffDataBase buffData)
    {
        enemyData?.HPAndBuffData.AddBuff(buffData);
    }
    public void AddBuffToAllEnemies(Func<BuffDataBase> buffDataCtor)
    {
        foreach (var enemyData in EnemyList)
        {
            enemyData.HPAndBuffData.AddBuff(buffDataCtor());
        }
    }

    public void AddBlockToPlayer(int addedBlock)
    {
        PlayerHPAndBuffData.Block.Value += addedBlock;
    }

    public void AddTempToDiscard(Func<CardDataBase> toAddCtor)
    {
        var toAdd = toAddCtor();
        toAdd.IsTemporary = true;
        DiscardList.MyAdd(toAdd);
    }
    public void AddTempToDraw(Func<CardDataBase> toAddCtor)
    {
        var toAdd = toAddCtor();
        toAdd.IsTemporary = true;
        var drawIndex = UnityEngine.Random.Range(0, DrawList.Count);
        DrawList.MyInsert(drawIndex, toAdd);
    }

    public void OpenDiscardOnceClick(Action<CardDataBase> onClick)
    {
        if(DiscardList.Count == 0)
            return;
        OnOpenDiscardOnceClick?.Invoke(DiscardList, onClick);
    }

    public void ExhaustHandCardBy(Func<CardDataBase, bool> match)
    {
        HandList.Where(match).ToList().ForEach(ExhaustOne);
    }
    
    public int DaJiCount => CollectAllCards().Count(card => card.Config.name.Contains("打击"));
    #endregion
    
}

