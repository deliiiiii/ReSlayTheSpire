using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;

public partial class Battle
{
    public EPlayerJob Job = EPlayerJob.ZhanShi;
    public MyList<CardData> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData?> BottleList = [];
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);
    public WeatherData WeatherData = new();
    public MapData MapData = new();
    
    public BattleFSM BattleFSM;
    public Game Game => FSM.Outer;
    
    public Battle()
    {
        Game.PlayerName = "DELI in battle";
        
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for (int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(new CardData(pair.Key.ID));
        });

        BattleFSM = new BattleFSM { Outer = this };
        BattleFSM.Launch<BothTurn>();
    }

    // public override void UnInit()
    // {
    //     DeckList.MyClear();
    // }
    //
    // public override void Tick(float dt)
    // {
    //     base.Tick(dt);
    //     InBattleTime.Value += dt;
    // }
}
public class BattleFSM : FSM2<Battle, BattleFSM, BattleState>;

public abstract class BattleState : StateBase<BattleFSM>;
public class BattleSelectLastBuff : BattleState;
public class BattleLose : BattleState;
public class BattleWin : BattleState;
public partial class BothTurn : BattleState;