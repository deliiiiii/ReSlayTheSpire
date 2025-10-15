using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RSTS;

[Serializable]
public class CardData(CardConfigMulti config)
{
    public int UpgradeLevel;
    public CardConfigMulti Config = config;

    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    public bool CanUpgrade => UpgradeLevel < Config.Upgrades.Count - 1;
}

[Serializable]
public class CardDataHolder
{
    public MyList<CardData> DeckList = [];
    public MyList<CardData> HandList = [];
    public MyList<CardData> DrawList = [];
    public MyList<CardData> DiscardList = [];
    public MyList<CardData> ExhaustList = [];
    public int Energy;
    
    public void OnEnterBothTurn()
    {
        HandList.MyClear();
        DrawList.MyClear();
        DrawList.MyAddRange(DeckList);
        DrawList.Shuffle();
        DiscardList.MyClear();
        ExhaustList.MyClear();
    }

    public void InitDeck(EPlayerJob job)
    {
        var config = RefPoolMulti<CardListConfigMulti>.Acquire().First(c => c.Job == job);
        config.InitialCards.ForEach(c =>
        {
            DeckList.MyAdd(new CardData(c));
        });
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

    public bool TryYield(CardData toYield, out string failReason)
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

    void Yield(CardData toYield)
    {
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
}