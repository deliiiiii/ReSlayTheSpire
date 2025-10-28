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

    public event Action? OnPlayerLoseHP;
    
    public BothTurnData(BattleData battleData, MyFSM<EBothTurn> fsm)
    {
        this.battleData = battleData;
        PlayerHPAndBuffData = new HPAndBuffData
        {
            CurHP = battleData.CurHP,
            MaxHP = battleData.MaxHP,
            Block = new Observable<int>(0)
        };
        PlayerHPAndBuffData.CurHP.OnValueChangedFull += (oldV, newV) =>
        {
            if (newV < oldV)
            {
                loseHpCount++;
                OnPlayerLoseHP?.Invoke();
            }
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
        
        
        fsm.GetState(EBothTurn.PlayerTurnEnd).OnEnter += () =>
        {
            HandList.ForEach(cardData => cardData.OnPlayerTurnEnd(this));
            DiscardAllHand();
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
    
    public string CurContentWithKeywords(CardDataBase cardData)
    {
        var replacerList = new List<string>();
        cardData.CurDes.EmbedTypes.ForEach(embedType =>
        {
            replacerList.Add(embedType switch
            {
                IEmbedNotChange notChange => notChange.GetNotChangeString(),
                EmbedCard6 => cardData.GetModify<AttackModifyCard6>(this).BaseAtkAddByDaJi.ToString(),
                // EmbedCard12 => cardData.GetModify<AttackModifyCard12>(this).AtkByBlock.ToString(),
                EmbedCard19 => cardData.GetModify<AttackModifyCard19>(this).BaseAtkAddByUse.ToString(),
                EmbedCard28 => cardData.GetModify<AttackModifyCard28>(this).AtkTimeByExhaust.ToString(),
                EmbedAttack attack => 
                    GetAttackValue(PlayerHPAndBuffData, cardData.Target?.HPAndBuffData, attack.AttackValue
                    , cardData.GetModifyList(this)).ToString(),
                _ => "NaN!"
            });
        });
        return cardData.CurUpgradeInfo.ContentWithKeywords(replacerList);
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
    
    public void LoseEnergy(int lose) => CurEnergy.Value -= lose;

    public void UseEnergy(int use) => CurEnergy.Value -= use;

    public void GainEnergy(int gain) => CurEnergy.Value += gain;

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
        var cost = GetEnergy(toYield);
        if (CurEnergy < cost)
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
            DiscardList.MyAdd(card);
        });
        HandList.MyClear();
    }

    public string UIGetEnergy(CardDataBase cardData)
    {
        return cardData.CurCostInfo switch
        {
            CardCostNumber number when cardData is Card24 => Math.Max(0, number.Cost - loseHpCount).ToString(),
            CardCostNumber number => number.Cost.ToString(),
            CardCostX => "X",
            CardCostNone or _ => "",
        };
    }
    public int GetEnergy(CardDataBase cardData)
    {
        int costEnergy = cardData.CurCostInfo switch
        {
            CardCostNumber number when cardData is Card24 => Math.Max(0, number.Cost - loseHpCount),
            CardCostNumber number => number.Cost,
            CardCostX => CurEnergy,
            CardCostNone or _ => 0,
        };
        return costEnergy;
    }
    public async UniTask YieldAsync(CardDataBase toYield)
    {
        int cost = GetEnergy(toYield);
        UseEnergy(cost);
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
        await toYield.YieldAsync(this, cost);
        if(EnemyList.Count == 0)
        {
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Win);
        }
    }
    
    public void ExhaustOne(CardDataBase toExhaust)
    {
        HandList.MyRemove(toExhaust);
        ExhaustList.MyAdd(toExhaust);
    }

    void TemporaryRemove(CardDataBase toRemove) => HandList.MyRemove(toRemove);


    void RefillDrawList()
    {
        DrawList.MyAddRange(DiscardList);
        DiscardList.MyClear();
        DrawList.Shuffle();
    }
    
    #region yield effect

    public void AttackEnemyWithResult(EnemyDataBase? enemyData, int baseAtk, out List<AttackResultBase> resultList
        , List<AttackModifyBase>? modifyList = null)
    {
        resultList = [];
        if (enemyData == null)
            return;
        Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out resultList, modifyList);
        if(resultList.OfType<AttackResultDie>().Any())
            EnemyList.MyRemove(enemyData);
    }
    // 使用攻击牌攻击的伤害
    public void AttackEnemy(EnemyDataBase? enemyData, int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        if (enemyData == null)
            return;
        Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out var resultList, modifyList);
        if(resultList.OfType<AttackResultDie>().Any())
            EnemyList.MyRemove(enemyData);
    }
    public async Task AttackEnemyMultiTimesAsync(EnemyDataBase? enemyData, int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        for (int t = 0; t < times; t++)
        {
            AttackEnemy(enemyData, baseAtk, modifyList);
            await Task.Delay(300);
        }
    }
    
    public void AttackEnemyRandomly(int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        if(EnemyList.Count == 0)
            return;
        var enemyData = EnemyList.RandomItem();
        AttackEnemy(enemyData, baseAtk, modifyList);
    }
    public async UniTask AttackEnemyRandomlyMultiTimesAsync(int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        for (int t = 0; t < times; t++)
        {
            AttackEnemyRandomly(baseAtk, modifyList);
            await UniTask.Delay(300);
        }
    }

    public void AttackAllEnemies(int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        var toRemoveList = new List<EnemyDataBase>();
        EnemyList.ForEach(enemyData =>
        {
            Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out var resultList, modifyList);
            if (resultList.OfType<AttackResultDie>().Any())
            {
                toRemoveList.Add(enemyData);
            }
        });
        toRemoveList.ForEach(toRemove => EnemyList.MyRemove(toRemove));
    }
    
    public void AttackAllEnemiesWithResult(int baseAtk, out List<AttackResultBase> allResult
        , List<AttackModifyBase>? modifyList = null)
    {
        var ret = new List<AttackResultBase>();
        var toRemoveList = new List<EnemyDataBase>();
        EnemyList.ForEach(enemyData =>
        {
            Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, out var resultList, modifyList);
            ret.AddRange(resultList);
            if (resultList.OfType<AttackResultDie>().Any())
            {
                toRemoveList.Add(enemyData);
            }
        });
        toRemoveList.ForEach(toRemove => EnemyList.MyRemove(toRemove));
        allResult = ret;
    }
    
    public async UniTask AttackAllEnemiesMultiTimesAsync(int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        for (int t = 0; t < times; t++)
        {
            AttackAllEnemies(baseAtk, modifyList);
            await UniTask.Delay(300);
        }
    }
    
    public void AttackPlayerFromEnemy(EnemyDataBase enemyData, int baseAtk)
    {
        Attack(enemyData.HPAndBuffData, PlayerHPAndBuffData, baseAtk, out var resultList);
        if (resultList.OfType<AttackResultDie>().Any())
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Lose);
    }

    public void GainMaxHP(int addedMaxHP)
    {
        PlayerMaxHP.Value += addedMaxHP;
        PlayerCurHP.Value += addedMaxHP;
    }
    
    public void GainCurHP(int addedCurHP)
    {
        // PlayerHPAndBuffData.
        PlayerCurHP.Value = Math.Clamp(PlayerCurHP + addedCurHP, PlayerCurHP, PlayerMaxHP);
    }
    
    public void BurnPlayer(int atk)
    {
        if(PlayerBlock >= atk)
        {
            PlayerBlock.Value -= atk;
            return;
        }
        var realBurn = atk - PlayerBlock;
        PlayerBlock.Value = 0;
        LoseHPToPlayer(realBurn);
    }
    
    public void LoseHPToPlayer(int loseHP)
    {
        PlayerCurHP.Value -= loseHP;
        if (PlayerCurHP <= 0)
            MyFSM.EnterState(BattleStateWrap.One, EBattleState.Lose);
    }
    
    int GetAttackValue(HPAndBuffData from, HPAndBuffData? to, int baseAtk, List<AttackModifyBase>? modifyList = null)
    {
        int strengthMultiFromCard4 = modifyList?.OfType<AttackModifyCard4>().FirstOrDefault()?.StrengthMulti ?? 1;
        baseAtk = 
            modifyList?.OfType<AttackModifyCard12>().FirstOrDefault()?.AtkByBlock ??
                  baseAtk;

        int baseCardAdd =
            modifyList?.OfType<AttackModifyCard6>().FirstOrDefault()?.BaseAtkAddByDaJi ??
            modifyList?.OfType<AttackModifyCard19>().FirstOrDefault()?.BaseAtkAddByUse ??
            0;
        
        var baseBuffAdd = from.GetAtkBaseAddSum(buff =>
        {
            if(buff is BuffDataStrength)
                return buff.GetAtkBaseAdd() * strengthMultiFromCard4;
            return buff.GetAtkBaseAdd();
        });
        var finalMulFrom = from.GetFromAtkFinalMulti();
        var finalMulTo = to?.GetToAtkFinalMulti() ?? 1f;
        return Mathf.FloorToInt((baseAtk + baseCardAdd + baseBuffAdd) * finalMulFrom * finalMulTo);
    }

    void Attack(HPAndBuffData from, HPAndBuffData to, int baseAtk, out List<AttackResultBase> resultList
        , List<AttackModifyBase>? modifyList = null)
    {
        resultList = [];
        if (!to.CanBeSelected)
            return;
        // (atk + strength) * (1 - weak) * (1 + vulnerable) * (1 + backAttack)
        var finalAtk = GetAttackValue(from, to, baseAtk, modifyList);
        // 先减格挡
        if (to.Block.Value >= finalAtk)
        {
            to.Block.Value -= finalAtk;
            return;
        }
        finalAtk -= to.Block;
        to.Block.Value = 0;
        
        // 再减血
        resultList.Add(new AttackResultHurt(finalAtk));
        to.CurHP.Value -= finalAtk;
        if (to.CurHP <= 0)
        {
            resultList.Add(new AttackResultDie(-to.CurHP));
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
        PlayerBlock.Value += addedBlock;
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

    public IEnumerable<CardDataBase> ExhaustHandCardBy(Func<CardDataBase, bool> match)
    {
        return HandList.Where(match).ToList();
    }
    
    public int DaJiCount => CollectAllCards().Count(card => card.Config.name.Contains("打击"));
    int loseHpCount;
    #endregion

}

