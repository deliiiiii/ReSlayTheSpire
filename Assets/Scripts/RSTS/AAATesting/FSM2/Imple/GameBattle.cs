using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;

public partial class GameBattle : FSM2<GameBattle>
{
    public class Card
    {
        public int ID;
        public int UpgradeLevel;
    }
    
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
            // for (int i = 0; i < pair.Value; i++)
                // DeckList.MyAdd(new CardData(pair.Key.ID));
        });

        Launch<BattleBothTurn>();
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

public class BattleSelectLastBuff : GameBattle.IState
{
    public required GameBattle BelongFSM { get; set; }
}

public class BattleLose : GameBattle.IState
{
    public required GameBattle BelongFSM { get; set; }
}

public class BattleWin : GameBattle.IState
{
    public required GameBattle BelongFSM { get; set; }
}

public partial class BattleBothTurn : GameBattle.IState
{
    public required GameBattle BelongFSM { get; set; }
}