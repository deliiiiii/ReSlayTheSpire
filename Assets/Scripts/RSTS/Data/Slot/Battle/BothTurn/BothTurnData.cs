using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
[Serializable]
public class BothTurnData : IMyFSMArg
{
    [NonSerialized]BattleData battleData;
    public Observable<int> PlayerCurHP => battleData.CurHP;
    public Observable<int> PlayerMaxHP => battleData.MaxHP;
    public Observable<int> PlayerBlock;
    [SerializeReference]public List<BuffDataBase> BuffList = [];
    public bool HasBuff<T>(out T buffData) where T : BuffDataBase
    {
        buffData = BuffList.OfType<T>().First();
        return buffData == null;
    }
    
    public int TurnID;
    [SerializeReference]public MyList<EnemyDataBase> EnemyList = [];
    [SerializeReference]public YieldCardData YieldCardData;
    public YieldCardData CreateYieldCardData() => 
        YieldCardData = new YieldCardData();

    static BothTurnData()
    {
        MyFSM.OnRegister(BothTurnStateWrap.One, alwaysBind: BindBothTurn);
    }
    public BothTurnData(BattleData battleData)
    {
        this.battleData = battleData;
        PlayerBlock = new Observable<int>(0);
        TurnID = 0;
    }

    static void BindBothTurn(MyFSM<EBothTurn> fsm, BothTurnData bothTurnData)
    {
        fsm.GetState(EBothTurn.TurnStart).OnEnter += () =>
        {
            bothTurnData.PlayerBlock.Value = 0;
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
        HandList.MyClear();
        DrawList.MyClear();
        DiscardList.MyClear();
        ExhaustList.MyClear();
    }
    
    [SerializeReference]
    public MyList<CardDataBase> HandList = [];
    [SerializeReference]
    public MyList<CardDataBase> DrawList = [];
    [SerializeReference]
    public MyList<CardDataBase> DiscardList = [];
    [SerializeReference]
    public MyList<CardDataBase> ExhaustList = [];
    public int Energy;

    public bool DrawSome(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (!DrawOne())
                return false;
        }
        return true;
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
        if (Energy < toYield.CurCostInfo switch
            {
                CardCostNumber costNumber => costNumber.Cost,
                CardCostX => 0,
                CardCostNone or _ => int.MaxValue,
            })
        {
            failReason = "能量不足";
            return false;
        }

        if(toYield.ContainsKeyword(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        return true;
    }

    public void DiscardAllHand()
    {
        HandList.ForEach(card =>
        {
            if(card.ContainsKeyword(ECardKeyword.Ethereal))
                ExhaustList.MyAdd(card);
            else if(card.Config.Category != ECardCategory.State)
                DiscardList.MyAdd(card);
        });
        HandList.MyClear();
    }

    public void Yield(CardDataBase toYield)
    {
        toYield.Yield(this);
        if (toYield.ContainsKeyword(ECardKeyword.Exhaust))
        {
            ExhaustOne(toYield);
        }
        else
        {
            NormallyYieldOne(toYield);
        }
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

    void NormallyYieldOne(CardDataBase toYield)
    {
        HandList.MyRemove(toYield);
        DiscardList.MyAdd(toYield);
    }
    
    
    void RefillDrawList()
    {
        DrawList.MyAddRange(DiscardList);
        DiscardList.MyClear();
        DrawList.Shuffle();
    }
    
    #region yield effect
    // 使用攻击牌攻击的伤害
    public void AttackEnemy(EnemyDataBase enemyData, int damage)
    {
        enemyData.CurHP.Value -= damage;
        if (enemyData.CurHP < 0)
            EnemyList.MyRemove(enemyData);
    }
    
    public void AddBuffToEnemy(EnemyDataBase enemyData, BuffDataBase buffData)
    {
        enemyData.BuffCom.AddBuff(buffData);
    }

    public void PlayerAddBlock(int addedBlock)
    {
        PlayerBlock.Value += addedBlock;
    }
    #endregion
}