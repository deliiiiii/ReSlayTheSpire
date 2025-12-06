using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;

public class GameFSM : FSM2<object, GameState>
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;
    public GameFSM(object context = null!) : base(context)
    {
        Launch<GameChoosePlayer>();
    }
}

public abstract class GameState(GameFSM fsm) : GameFSM.StateBase(fsm);
public class GameChoosePlayer(GameFSM fsm) : GameState(fsm);
public class GameTitle(GameFSM fsm) : GameState(fsm);
public class GameBattle : GameState
{
    BattleFSM battleFSM;
    public GameBattle(GameFSM fsm) : base(fsm)
    {
        MyDebug.Log("Enter Battle State");
        battleFSM = new(fsm);
    }
}

public class BattleFSM : FSM2<GameFSM, BattleState>
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

    public BattleFSM(GameFSM fsm) : base(fsm)
    {
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for (int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(new CardData(pair.Key.ID));
        });
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
