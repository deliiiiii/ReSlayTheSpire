using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;

namespace RSTS;
[Serializable]
public class BothTurnData : FSM<BothTurnData, EBothTurnState, BattleData>
{
    #region 持久数据
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
    /// 第一个参数，弃牌堆；第二个参数，点击后的回调
    public event Action<List<CardInTurn>, Action<CardInTurn>>? OnOpenDiscardOnceClick;
    public event Action<List<CardInTurn>, int, Action<CardInTurn>>? OnOpenHandOnceClick;
    public event Action? OnPlayerLoseHP;
    #endregion
    
    [SubState<EBothTurnState>(EBothTurnState.PlayerYieldCard)]
    YieldCardData yieldCardData = null!;
    
    public BothTurnData(BattleData parent) : base(parent)
    {
        PlayerHPAndBuffData.CurHP = Parent.CurHP;
        PlayerHPAndBuffData.MaxHP = Parent.MaxHP;
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
        
        Parent.DeckList.ForEach(cardData =>
        {
            DrawList.MyAdd(CardInTurn.CreateByAttr(cardData.Config.ID, cardData));
        });
        DrawList.Shuffle();
        
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        CollectAllCards().ForEach(cardInTurn => cardInTurn.OnEnterBothTurn());

        
        GetState(EBothTurnState.GrossStart).OnEnter(() =>
        {
            EnterState(EBothTurnState.PlayerTurnStart);
        });
        
        GetState(EBothTurnState.PlayerTurnStart).OnEnter(() =>
        {
            TurnID++;
            PlayerBlock.Value = 0;
            LoseEnergy(CurEnergy.Value);
            GainEnergy(MaxEnergy);
            PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
            PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
            
            EnterState(EBothTurnState.PlayerDraw);
        });
        
        GetState(EBothTurnState.PlayerDraw).OnEnter(() =>
        {
            DrawSome(5);
            EnterState(EBothTurnState.PlayerYieldCard);
        });
        
        GetState(EBothTurnState.PlayerYieldCard)
            .OnEnter(() =>
            {
                yieldCardData = new(this);
                yieldCardData.Launch(EYieldCardState.None);
            })
            .OnExit(() =>
            {
                yieldCardData.Release();
                yieldCardData = null!;
            });
        
        GetState(EBothTurnState.PlayerTurnEnd).OnEnter(() =>
        {
            HandList.ForEach(cardInTurn => cardInTurn.OnPlayerTurnEnd(this));
            DiscardAllHand();
            PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
            PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
            EnterState(EBothTurnState.EnemyTurnStart);
        });
        
        GetState(EBothTurnState.EnemyTurnStart).OnEnter(() =>
        {
            EnemyList.ForEach(enemyData =>
            {
                enemyData.HPAndBuffData.Block.Value = 0;
                enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
                enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
            });

            EnterState(EBothTurnState.EnemyAction);
            // Func().Forget();
            // return;
            //
            // async UniTask Func()
            // {
            //     await UniTask.Delay(1000);
            //     MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.EnemyAction);
            // }
        });

        GetState(EBothTurnState.EnemyAction).OnEnter(() =>
        {
            Func().Forget();
            return;
            
            async UniTask Func()
            {
                foreach (var enemyData in EnemyList)
                {
                    var resultList = await enemyData.DoCurIntentionAsync(this);
                    if (resultList.AnyType<AttackResultDie>())
                    {
                        Parent.EnterState(EBattleState.Lose);
                        return;
                    }
                    await UniTask.Delay(200);
                }
                EnterState(EBothTurnState.EnemyTurnEnd);
            }
        });
        
        GetState(EBothTurnState.EnemyTurnEnd).OnEnter(() =>
        {
            EnemyList.ForEach(enemyData =>
            {
                enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
                enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
            });
            EnterState(EBothTurnState.PlayerTurnStart);
        });
    }
    protected override void UnInit()
    {
        CollectAllCards().ForEach(cardInTurn => cardInTurn.OnExitBothTurn());
        
        EnemyList.MyClear();
        
        HandList.MyClear();
        DrawList.MyClear();
        DiscardList.MyClear();
        ExhaustList.MyClear();
        
        PlayerHPAndBuffData.ClearBuff();
    }
    
    public string CurContentWithKeywords(CardInTurn cardInTurn)
    {
        var replacerList = new List<string>();
        cardInTurn.Parent.CurDes.EmbedTypes.ForEach(embedType =>
        {
            replacerList.Add(embedType switch
            {
                IEmbedNotChange notChange => notChange.GetNotChangeString(),
                EmbedCard6 => cardInTurn.GetModify<AttackModifyCard6>(this).BaseAtkAddByDaJi.ToString(),
                // EmbedCard12 => cardData.GetModify<AttackModifyCard12>(this).AtkByBlock.ToString(),
                EmbedCard19 => cardInTurn.GetModify<AttackModifyCard19>(this).BaseAtkAddByUse.ToString(),
                EmbedCard28 => cardInTurn.GetModify<AttackModifyCard28>(this).AtkTimeByExhaust.ToString(),
                EmbedAttack attack => 
                    GetAttackValue(PlayerHPAndBuffData, cardInTurn.Target?.HPAndBuffData, attack.AttackValue
                    , cardInTurn.GetModifyList(this)).ToString(),
                _ => "NaN!"
            });
        });
        return cardInTurn.Parent.CurUpgradeInfo.ContentWithKeywords(replacerList);
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
    public bool DrawSome(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (!DrawOne())
                return false;
        }
        return true;
    }

    public bool TryPullOneFromDraw(bool shouldRefill, out CardInTurn drawn)
    {
        drawn = null!;
        if (DrawList.Count == 0 && shouldRefill)
            RefillDrawList();
        if (DrawList.Count == 0)
        {
            Debug.LogError("没有牌可以抽了");
            return false;
        }
        drawn = DrawList[^1];
        DrawList.MyRemoveAt(DrawList.Count - 1);
        return true;
    }
    
    bool DrawOne()
    {
        var ret = TryPullOneFromDraw(shouldRefill: true, out var drawn);
        if(ret)
            HandList.MyAdd(drawn);
        return ret;
    }

    public bool TryYield(CardInTurn toYield, out string failReason)
    {
        failReason = string.Empty;
        if(toYield.Parent.ContainsKeyword(ECardKeyword.Unplayable))
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
            if (card.Parent.ContainsKeyword(ECardKeyword.Ethereal))
            {
                ExhaustList.MyAdd(card);
                return;
            }
            DiscardList.MyAdd(card);
        });
        HandList.MyClear();
    }

    public string UIGetEnergy(CardInTurn cardInTurn)
    {
        return cardInTurn.Parent.CurCostInfo switch
        {
            CardCostNumber number when cardInTurn is Card24 => Math.Max(0, number.Cost - loseHpCount).ToString(),
            CardCostNumber number => number.Cost.ToString(),
            CardCostX => "X",
            CardCostNone or _ => "",
        };
    }
    public int GetEnergy(CardInTurn cardInTurn)
    {
        int costEnergy = cardInTurn.Parent.CurCostInfo switch
        {
            CardCostNumber number when cardInTurn is Card24 => Math.Max(0, number.Cost - loseHpCount),
            CardCostNumber number => number.Cost,
            CardCostX => CurEnergy,
            CardCostNone or _ => 0,
        };
        return costEnergy;
    }
    public async UniTask YieldHandAsync(CardInTurn toYield
        , List<YieldModify>? modifyList = null)
    {
        yieldCardData.Release();
        int cost = GetEnergy(toYield);
        UseEnergy(cost);
        HandList.MyRemove(toYield);
        modifyList ??= [];
        await YieldInternal(toYield, cost, modifyList);
        toYield.Target = null;
        yieldCardData.Launch(EYieldCardState.None);
        if(EnemyList.Count == 0)
        {
            Parent.EnterState(EBattleState.Win);
        }
    }

    public async UniTask YieldBlindAsync(CardInTurn toYield
        , List<YieldModify>? modifyList = null)
    {
        modifyList ??= [];
        await YieldInternal(toYield, 0, modifyList);
    }

    async UniTask YieldInternal(CardInTurn toYield, int cost
        , List<YieldModify> modifyList)
    {
        if (toYield.Parent.Config.Category == ECardCategory.Ability)
        {
            // 打出能力牌，不会消耗
        }
        else if (toYield.Parent.ContainsKeyword(ECardKeyword.Exhaust) 
                 || modifyList.AnyType<YieldModifyForceExhaust>())
        {
            // 消耗
            ExhaustList.MyAdd(toYield);
        }
        else
        {
            // 正常打出
            DiscardList.MyAdd(toYield);
        }

        if (toYield.Parent.Config.Category == ECardCategory.Attack)
        {
            if (PlayerHPAndBuffData.HasBuff<BuffDataAttackGainBlock>(out var buff))
            {
                GainBlock(buff.StackCount);
            }
        }
        await toYield.YieldAsync(this, cost);
      }


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
            if (resultList.AnyType<AttackResultDie>())
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
    
    public void BurnPlayer(int atk)
    {
        if(PlayerBlock >= atk)
        {
            PlayerBlock.Value -= atk;
            return;
        }
        var realBurn = atk - PlayerBlock;
        PlayerBlock.Value = 0;
        LoseHP(realBurn);
    }
    
    public void AttackPlayerFromEnemy(EnemyDataBase enemyData, int baseAtk, out List<AttackResultBase> resultList
        , List<AttackModifyBase>? modifyList = null)
    {
        modifyList ??= [];
        Attack(enemyData.HPAndBuffData, PlayerHPAndBuffData, baseAtk, out resultList, modifyList);
        if (PlayerHPAndBuffData.HasBuff<BuffFlameBarrier>(out var buffFlameBarrier))
        {
            var flameDamage = buffFlameBarrier.StackCount;
            AttackEnemy(enemyData, flameDamage, [new AttackModifyFromBuff()]);
        }
    }

    public async UniTask<List<AttackResultBase>> AttackPlayerFromEnemyMultiTimesAsync(EnemyDataBase enemyData, int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        List<AttackResultBase> resultList = [];
        for (int t = 0; t < times; t++)
        {
            AttackPlayerFromEnemy(enemyData, baseAtk, out var curResultList, modifyList);
            resultList.AddRange(curResultList);
            await UniTask.Delay(300);
        }

        return resultList;
    }

    public void GainMaxHP(int addedMaxHP)
    {
        PlayerMaxHP.Value += addedMaxHP;
        PlayerCurHP.Value += addedMaxHP;
    }
    
    public void GainCurHP(int addedCurHP) 
        => PlayerCurHP.Value = Math.Clamp(PlayerCurHP + addedCurHP, PlayerCurHP, PlayerMaxHP);

    public void LoseHP(int loseHP)
    {
        PlayerCurHP.Value -= loseHP;
        if (PlayerCurHP <= 0)
            Parent.EnterState(EBattleState.Lose);
    }
    
    public void GainBlock(int addedBlock) => PlayerBlock.Value += addedBlock;
    public void LoseEnergy(int lose) => CurEnergy.Value -= lose;
    public void UseEnergy(int use) => CurEnergy.Value -= use;
    public void GainEnergy(int gain) => CurEnergy.Value += gain;
    
    int GetAttackValue(HPAndBuffData from, HPAndBuffData? to, int baseAtk, List<AttackModifyBase> modifyList)
    {
        if (modifyList.AnyType<AttackModifyFromBuff>())
        {
            return baseAtk;
        }
        int strengthMultiFromCard4 = modifyList.OfType<AttackModifyCard4>().FirstOrDefault()?.StrengthMulti ?? 1;
        baseAtk = 
            modifyList.OfType<AttackModifyCard12>().FirstOrDefault()?.AtkByBlock ??
                  baseAtk;

        int baseCardAdd =
            modifyList.OfType<AttackModifyCard6>().FirstOrDefault()?.BaseAtkAddByDaJi ??
            modifyList.OfType<AttackModifyCard19>().FirstOrDefault()?.BaseAtkAddByUse ??
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
        modifyList ??= [];
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

    public void AddTempToDiscard(Func<CardInTurn> toAddCtor)
    {
        var toAdd = toAddCtor();
        toAdd.IsTemporary = true;
        DiscardList.MyAdd(toAdd);
    }
    public void AddTempToDraw(Func<CardInTurn> toAddCtor)
    {
        var toAdd = toAddCtor();
        toAdd.IsTemporary = true;
        var drawIndex = UnityEngine.Random.Range(0, DrawList.Count);
        DrawList.MyInsert(drawIndex, toAdd);
    }

    public void OpenDiscardOnceClick(Action<CardInTurn> onConfirm)
    {
        if(DiscardList.Count == 0)
            return;
        OnOpenDiscardOnceClick?.Invoke(DiscardList, onConfirm);
    }
    
    public void OpenHandCardOnceClick(int selectCount, Func<CardInTurn, bool> filter, Action<CardInTurn> onConfirm)
    {
        var filtered = HandList.Where(filter).ToList();
        if (filtered.Count == 0)
            return;
        // 选择随机一个手牌。。。 
        var selected = filtered.RandomItem();
        MyDebug.Log($"选手牌：是{selected.Parent.Config.name}");
        onConfirm(selected);
        // TODO UI选择
        // OnOpenHandOnceClick?.Invoke(filtered, selectCount, onConfirm);
    }

    
    public int DaJiCount => CollectAllCards().Count(card => card.Parent.Config.name.Contains("打击"));
    int loseHpCount;
    #endregion

}

