using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;
public partial class Battle
{
    public EPlayerJob Job = EPlayerJob.ZhanShi;
    public MyList<Card> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData> BottleList = [];
    public HPAndBuffData PlayerHPAndBuff = new();
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);
    public MapData MapData = new();
    
    public override void OnEnter()
    {
        BelongFSM.PlayerName = "DELI in battle";
        
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for (int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(Card.Create(pair.Key.ID));
        });
        
        for (int i = 0; i < 3; i++)
        {
            BottleList.MyAdd(new BottleNone());
        }
        PlayerHPAndBuff.CurHP.Value = PlayerHPAndBuff.MaxHP.Value = config.MaxHP;
        Coin.Value = 99;

        Launch<BattleSelectLastBuff>();
    }

    public override void OnExit()
    {
        DeckList.MyClear();
    }
    public override void OnUpdate(float dt)
    {
        InBattleTime.Value += dt;
    }
}
[Serializable]
public class BattleSelectLastBuff : FSMState<Battle, BattleSelectLastBuff>;
[Serializable]
public partial class BothTurn : FSMState<Battle, BothTurn>;
[Serializable]
public class BattleLose : FSMState<Battle, BattleLose>;
[Serializable]
public class BattleWin : FSMState<Battle, BattleWin>;