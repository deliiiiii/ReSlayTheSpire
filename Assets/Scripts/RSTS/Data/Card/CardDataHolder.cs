using System;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;
using UnityEngine;
namespace RSTS;
[Serializable]
public class CardDataHolder
{
    [SerializeReference]
    public MyList<CardDataBase> DeckList = [];
    [SerializeReference]
    public MyList<CardDataBase> HandList = [];
    [SerializeReference]
    public MyList<CardDataBase> DrawList = [];
    [SerializeReference]
    public MyList<CardDataBase> DiscardList = [];
    [SerializeReference]
    public MyList<CardDataBase> ExhaustList = [];
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
        config.InitialCardDic.ForEach(pair =>
        {
            for(int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(CardDataBase.CreateCard(pair.Key.ID));
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