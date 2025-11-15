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
    [NonSerialized] GameData gameData;
    
    public EPlayerJob Job;
    
    public MyList<CardDataBase> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData?> BottleList = [];
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);

    [SerializeReference] BothTurnData bothTurnData;
    public BothTurnData CreateBothTurnData(MyFSM<EBothTurn> fsm) => bothTurnData = new (this, fsm);
    #region Init, Launch
    public BattleData(GameData gameData, EPlayerJob job)
    {
        this.gameData = gameData;
        Job = job;
    }
    public void Launch()
    {
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for(int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(CardDataBase.CreateData(pair.Key.ID));
        });
        for (int i = 0; i < 3; i++)
        {
            BottleList.MyAdd(null);
        }
        CurHP.Value = MaxHP.Value = config.MaxHP;
        Coin.Value = 99;
    }
    public void UnInit()
    {
        DeckList.MyClear();
    }
    #endregion
    
    // #region MapData
    // [SerializeReference]MapData mapData;
    // #endregion
}