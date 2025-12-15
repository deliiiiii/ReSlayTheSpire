using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS.CDMV;
using Sirenix.Utilities;
using UnityEngine;

namespace RSTS;
public partial class Battle : FSM2<Battle>
{
    public EPlayerJob Job = EPlayerJob.ZhanShi;
    public MyList<Card> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData?> BottleList = [];
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);
    public MapData MapData = new();
    
    public void OnEnter()
    {
        BelongFSM.PlayerName = "DELI in battle";
        
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for (int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(Card.Create(this, pair.Key.ID));
        });
        
        for (int i = 0; i < 3; i++)
        {
            BottleList.MyAdd(null);
        }
        CurHP.Value = MaxHP.Value = config.MaxHP;
        Coin.Value = 99;

        Launch<BattleSelectLastBuff>();
    }

    public void OnExit()
    {
        DeckList.MyClear();
    }
    public void OnUpdate(float dt)
    {
        InBattleTime.Value += dt;
    }
}
[Serializable]
public class BattleSelectLastBuff : Battle.IState
{
    public required Battle BelongFSM { get; set; }
}
[Serializable]
public partial class BothTurn : Battle.IState
{
    public required Battle BelongFSM { get; set; }
}
[Serializable]
public class BattleLose : Battle.IState
{
    public required Battle BelongFSM { get; set; }
}
[Serializable]
public class BattleWin : Battle.IState
{
    public required Battle BelongFSM { get; set; }
}