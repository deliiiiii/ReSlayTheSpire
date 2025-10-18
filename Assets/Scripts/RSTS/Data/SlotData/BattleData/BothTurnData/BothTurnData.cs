using System;
using RSTS;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

[Serializable]
public class BothTurnData : IMyFSMArg
{
    public int PlayerDefend;
    public int TurnID;
    [SerializeReference]public MyList<EnemyDataBase> EnemyList = [];
    
    public BothTurnData(MyList<CardDataBase> deckList, Action<BothTurnData>? onCreate)
    {
        onCreate?.Invoke(this);
        PlayerDefend = TurnID = 0;
        EnemyList.MyClear();
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
        EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
        
        HandList.MyClear();
        DrawList.MyClear();
        DrawList.MyAddRange(deckList);
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
        if (Energy < toYield.CurUpgradeInfo.CostInfo switch
            {
                CardCostNumber costNumber => costNumber.Cost,
                CardCostX => 0,
                CardCostNone or _ => int.MaxValue,
            })
        {
            failReason = "能量不足";
            return false;
        }

        if(toYield.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Unplayable))
        {
            failReason = "该牌无法打出";
            return false;
        }
        Yield(toYield);
        return true;
    }

    public void DiscardAllHand()
    {
        HandList.ForEach(c =>
        {
            if(c.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Ethereal))
                ExhaustList.MyAdd(c);
            else if(c.Config.Category != ECardCategory.State)
                DiscardList.MyAdd(c);
        });
        HandList.MyClear();
    }

    void Yield(CardDataBase toYield)
    {
        toYield.Yield(this);
        HandList.MyRemove(toYield);
        if (toYield.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Exhaust))
        {
            ExhaustList.MyAdd(toYield);
        }
        else
        {
            DiscardList.MyAdd(toYield);
        }
    }
    
    
    void RefillDrawList()
    {
        DrawList.MyAddRange(DiscardList);
        DiscardList.MyClear();
        DrawList.Shuffle();
    }
    
    [SerializeReference]YieldCardData yieldCardData;
    public YieldCardData CreateYieldCardData() => 
        yieldCardData = new YieldCardData();
}