using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;
using UnityEngine;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
[Serializable]
public class BattleData : IMyFSMArg
{
    [NonSerialized] SlotData slotData;
    
    [SerializeReference]
    public MyList<CardDataBase> DeckList = [];
    
    public List<ItemData> ItemList = [];
    public int CurHP;
    public int MaxHP;
    public int Coin;
    public EPlayerJob Job;
    public List<BottleData> BottleList = [];
    public float InBattleTime;
    
    #region Init, Launch
    public BattleData(SlotData slotData, EPlayerJob job)
    {
        this.slotData = slotData;
        Job = job;
    }
    public void Launch()
    {
        MyDebug.Log("BattleData Launch");
        var config = RefPoolMulti<CardListConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for(int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(CardDataBase.CreateCard(pair.Key.ID));
        });
    }
    public void UnInit()
    {
        MyDebug.Log("BattleData UnInit");
        DeckList.MyClear();
    }
    #endregion

    #region BothTurnData
    [SerializeReference]BothTurnData bothTurnData;
    public BothTurnData CreateBothTurnData()
        => bothTurnData = new BothTurnData(this);
    #endregion
    [SerializeReference]MapData mapData;
}