using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RSTS;

public partial class BothTurn : FSM2<BothTurn>
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
    public int LoseHpCount;
    
    public MyList<Card> HandList = [];
    public MyList<Card> DrawList = [];
    public MyList<Card> DiscardList = [];
    public MyList<Card> ExhaustList = [];
#pragma warning disable CS0067 // 事件从未使用过
    /// 第一个参数，弃牌堆；第二个参数，点击后的回调
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
                LoseHpCount++;
                OnPlayerLoseHP?.Invoke();
            }
        };
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        TurnID = 0;
        
        BelongFSM.DeckList.ForEach(card =>
        {
            card.EnterTurn(this);
            DrawList.MyAdd(card);
        });
        DrawList.Shuffle();
    }
    public void OnExit()
    {
        DrawList.MyClear();
        BelongFSM.DeckList.ForEach(card => card.ExitTurn(this));
        EnemyList.MyClear();
        
        HandList.MyClear();
        DiscardList.MyClear();
        ExhaustList.MyClear();
        
        PlayerHPAndBuffData.ClearBuff();
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
    public bool DrawSome(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (!DrawOne())
                return false;
        }
        return true;
    }
    public bool TryPullOneFromDraw(bool shouldRefill, out Card drawn)
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
    void RefillDrawList()
    {
        DrawList.MyAddRange(DiscardList);
        DiscardList.MyClear();
        DrawList.Shuffle();
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
    public async UniTask YieldHandAsync(Card card, EnemyDataBase? target, List<YieldModifyBase>? modifyList = null)
    {
        modifyList ??= [];
        
        int cost = card[this].Energy;
        if (modifyList.AnyType<YieldModifyFromDraw>())
            cost = 0;
        UseEnergy(cost);
        
        if(!modifyList.AnyType<YieldModifyFromDraw>())
            HandList.MyRemove(card);
        
        // 打出能力牌，不会消耗
        if (card.Config.Category == ECardCategory.Ability)
        {
        }
        // 消耗
        else if (card.ContainsKeyword(ECardKeyword.Exhaust)
                 || modifyList.AnyType<YieldModifyForceExhaust>())
        {
            ExhaustList.MyAdd(card);
        }
        // 正常打出
        else
        {
            DiscardList.MyAdd(card);
        }

        if (card.Config.Category == ECardCategory.Attack)
        {
            if (PlayerHPAndBuffData.HasBuff<BuffDataAttackGainBlock>(out var buff))
            {
                GainBlock(buff.StackCount);
            }
        }
        await card[this].YieldAsync(cost, target);
        if(EnemyList.Count == 0)
        {
            BelongFSM.EnterState<BattleWin>();
        }
    }
    
    #region yield effect
    EnemyDataBase? RanEnemy() => EnemyList.Count == 0 ? null : EnemyList.RandomItem();
    public List<AttackResultBase> AttackEnemyWithResult(EnemyDataBase? enemyData, int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        enemyData ??= RanEnemy();
        if (enemyData == null)
            return [];
        var ret = Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, modifyList);
        if(ret.OfType<AttackResultDie>().Any())
            EnemyList.MyRemove(enemyData);
        return ret;
    }
    // 使用攻击牌攻击的伤害
    public List<AttackResultBase> AttackEnemy(EnemyDataBase? enemyData, int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        enemyData ??= RanEnemy();
        if (enemyData == null)
            return [];
        var ret = Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, modifyList);
        if(ret.OfType<AttackResultDie>().Any())
            EnemyList.MyRemove(enemyData);
        return ret;
    }
    public async UniTask AttackEnemyMultiTimesAsync(EnemyDataBase? enemyData, int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        for (int t = 0; t < times; t++)
        {
            AttackEnemy(enemyData, baseAtk, modifyList);
            await UniTask.Delay(300);
        }
    }
    public void AttackEnemyRandomly(int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        var enemyData = RanEnemy();
        if (enemyData == null)
            return;
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
    public List<AttackResultBase> AttackAllEnemies(int baseAtk, List<AttackModifyBase>? modifyList = null)
    {
        var ret = new List<AttackResultBase>();
        var toRemoveList = new List<EnemyDataBase>();
        EnemyList.ForEach(enemyData =>
        {
            ret.AddRange(Attack(PlayerHPAndBuffData, enemyData.HPAndBuffData, baseAtk, modifyList));
            if (ret.AnyType<AttackResultDie>())
            {
                toRemoveList.Add(enemyData);
            }
        });
        toRemoveList.ForEach(toRemove => EnemyList.MyRemove(toRemove));
        return ret;
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
    public List<AttackResultBase> AttackPlayerFromEnemy(EnemyDataBase enemyData, int baseAtk
        , List<AttackModifyBase>? modifyList = null)
    {
        modifyList ??= [];
        var ret = Attack(enemyData.HPAndBuffData, PlayerHPAndBuffData, baseAtk, modifyList);
        if (PlayerHPAndBuffData.HasBuff<BuffFlameBarrier>(out var buffFlameBarrier))
        {
            var flameDamage = buffFlameBarrier.StackCount;
            AttackEnemy(enemyData, flameDamage, [new AttackModifyFromBuff()]);
        }

        return ret;
    }
    public async UniTask<List<AttackResultBase>> AttackPlayerFromEnemyMultiTimesAsync(EnemyDataBase enemyData, int baseAtk, int times
        , List<AttackModifyBase>? modifyList = null)
    {
        List<AttackResultBase> ret = [];
        for (int t = 0; t < times; t++)
        {
            var retTemp = AttackPlayerFromEnemy(enemyData, baseAtk, modifyList);
            ret.AddRange(retTemp);
            await UniTask.Delay(300);
        }

        return ret;
    }

    public void GainMaxHP(int addedMaxHP)
    {
        PlayerMaxHP.Value += addedMaxHP;
        PlayerCurHP.Value += addedMaxHP;
    }
    public void GainCurHP(int addedCurHP) => PlayerCurHP.Value = Math.Clamp(PlayerCurHP + addedCurHP, PlayerCurHP, PlayerMaxHP);
    public void LoseHP(int loseHP)
    {
        PlayerCurHP.Value -= loseHP;
        if (PlayerCurHP <= 0)
            BelongFSM.EnterState<BattleLose>();
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
        baseAtk = modifyList.OfType<AttackModifyCard12>().FirstOrDefault()?.AtkByBlock ??
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
    List<AttackResultBase> Attack(HPAndBuffData from, HPAndBuffData to, int baseAtk,
        List<AttackModifyBase>? modifyList = null)
    {
        List<AttackResultBase> ret = [];
        modifyList ??= [];
        if (!to.CanBeSelected)
            return ret;
        // (atk + strength) * (1 - weak) * (1 + vulnerable) * (1 + backAttack)
        var finalAtk = GetAttackValue(from, to, baseAtk, modifyList);
        // 先减格挡
        if (to.Block.Value >= finalAtk)
        {
            to.Block.Value -= finalAtk;
            return ret;
        }
        finalAtk -= to.Block;
        to.Block.Value = 0;
        
        // 再减血
        ret.Add(new AttackResultHurt(finalAtk));
        to.CurHP.Value -= finalAtk;
        if (to.CurHP <= 0)
        {
            ret.Add(new AttackResultDie(-to.CurHP));
        }
        return ret;
    }
    
    public void AddBuffToPlayer(BuffDataBase buffData)
    {
        PlayerHPAndBuffData.AddBuff(buffData);
    }
    public void AddBuffToEnemy(EnemyDataBase? enemyData, BuffDataBase buffData)
    {
        enemyData?.HPAndBuffData.AddBuff(buffData);
    }
    public void AddBuffToAllEnemies(BuffDataBase buffData)
    {
        foreach (var enemyData in EnemyList)
        {
            enemyData.HPAndBuffData.AddBuff(buffData);
        }
    }

    public void AddTempToDiscard(Card card)
    {
        card[this].IsTemporary = true;
        DiscardList.MyAdd(card);
    }
    public void AddTempToDraw(Card card)
    {
        card[this].IsTemporary = true;
        var drawIndex = UnityEngine.Random.Range(0, DrawList.Count);
        DrawList.MyInsert(drawIndex, card);
    }

    public void OpenDiscardOnceClick(Action<Card> onConfirm)
    {
        if(DiscardList.Count == 0)
            return;
        OnOpenDiscardOnceClick?.Invoke(DiscardList, onConfirm);
    }
    public void OpenHandCardOnceClick(int selectCount, Func<Card, bool> filter, Action<Card> onConfirm)
    {
        var filtered = HandList.Where(filter).ToList();
        if (filtered.Count == 0)
            return;
        // 选择随机一个手牌。。。 
        var selected = filtered.RandomItem();
        MyDebug.Log($"选手牌：是{selected.Config.name}");
        onConfirm(selected);
        // TODO UI选择
        // OnOpenHandOnceClick?.Invoke(filtered, selectCount, onConfirm);
    }

    
    public int DaJiCount => CollectAllCards().Count(card => card.Config.name.Contains("打击"));
    #endregion
}

[Serializable]
public class BothTurnGrossStart : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        BelongFSM.EnterState<BothTurnPlayerTurnStart>();
    }
}
[Serializable]
public class BothTurnPlayerTurnStart : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        BelongFSM.TurnID++;
        BelongFSM.PlayerBlock.Value = 0;
        BelongFSM.LoseEnergy(BelongFSM.CurEnergy.Value);
        BelongFSM.GainEnergy(BelongFSM.MaxEnergy);
        BelongFSM.PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
        BelongFSM.PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
            
        BelongFSM.EnterState<BothTurnPlayerDraw>();
    }
}
[Serializable]
public class BothTurnPlayerDraw : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        BelongFSM.DrawSome(5);
        BelongFSM.EnterState<YieldCard>();
    }
}

[Serializable]
public partial class YieldCard : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
}
[Serializable] 
public class BothTurnPlayerTurnEnd : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }

    public void OnEnter()
    {
        BelongFSM.DiscardAllHand();
        BelongFSM.PlayerHPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
        BelongFSM.PlayerHPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
        BelongFSM.EnterState<BothTurnEnemyTurnStart>();
    }
}

[Serializable]
public class BothTurnEnemyTurnStart : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        BelongFSM.EnemyList.ForEach(enemyData =>
        {
            enemyData.HPAndBuffData.Block.Value = 0;
            enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnStart);
            enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnStart);
        });
        BelongFSM.EnterState<BothTurnEnemyAction>();
    }
}
[Serializable]
public class BothTurnEnemyAction : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        Func().Forget();
        return;
            
        async UniTask Func()
        {
            foreach (var enemyData in BelongFSM.EnemyList)
            {
                var resultList = await enemyData.DoCurIntentionAsync(BelongFSM);
                if (resultList.AnyType<AttackResultDie>())
                {
                    BelongFSM.BelongFSM.EnterState<BattleLose>();
                    return;
                }
                await UniTask.Delay(200);
            }
            BelongFSM.EnterState<BothTurnEnemyTurnEnd>();
        }
    }
}
[Serializable]
public class BothTurnEnemyTurnEnd : BothTurn.IState
{
    public required BothTurn BelongFSM { get; set; }
    public void OnEnter()
    {
        BelongFSM.EnemyList.ForEach(enemyData =>
        {
            enemyData.HPAndBuffData.UseABuff(EBuffUseTime.TurnEnd);
            enemyData.HPAndBuffData.DisposeABuff(EBuffDisposeTime.TurnEnd);
        });
        BelongFSM.EnterState<BothTurnPlayerTurnStart>();
    }
}